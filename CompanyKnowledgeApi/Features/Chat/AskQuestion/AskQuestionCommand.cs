using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Ai.Chat;
using CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace CompanyKnowledgeApi.Features.Chat.AskQuestion;

public sealed class AskQuestionCommand(
    AppDbContext dbContext,
    IEmbeddingService embeddingService,
    IChatCompletionService chatCompletionService,
    IValidator<AskQuestionModel> validator)
    : ICommand<AskQuestionModel, IResult>, IScopedService
{
    private const int DefaultTopK = 5;

    public async Task<IResult> Handle(AskQuestionModel model, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var session = await GetOrCreateSessionAsync(model, cancellationToken);
        var topK = model.TopK ?? DefaultTopK;
        var embeddings = await embeddingService.EmbedAsync([model.Question], cancellationToken);
        var questionEmbedding = new Vector(embeddings[0]);

        var chunks = await dbContext.DocumentChunks
            .AsNoTracking()
            .Where(chunk => chunk.Embedding != null)
            .Where(chunk => chunk.Document.Status != DocumentStatus.Deleted)
            .OrderBy(chunk => chunk.Embedding!.CosineDistance(questionEmbedding))
            .Take(topK)
            .Select(chunk => new
            {
                chunk.DocumentId,
                DocumentName = chunk.Document.FileName,
                ChunkId = chunk.Id,
                chunk.Content,
                chunk.PageNumber,
                chunk.ChunkIndex,
                Distance = chunk.Embedding!.CosineDistance(questionEmbedding)
            })
            .ToListAsync(cancellationToken);

        if (chunks.Count == 0)
        {
            const string noSourceAnswer = "Bu soruya yanıt verebilmek için ilgili kaynak bulunamadı.";
            await SaveConversationAsync(session, model.Question, noSourceAnswer, [], cancellationToken);

            return Results.Ok(new AskQuestionResponse(
                SessionId: session.Id,
                Answer: noSourceAnswer,
                Sources: []));
        }

        var sources = chunks
            .Select(chunk => new AskQuestionSource(
                DocumentId: chunk.DocumentId,
                DocumentName: chunk.DocumentName,
                ChunkId: chunk.ChunkId,
                Content: chunk.Content,
                PageNumber: chunk.PageNumber,
                ChunkIndex: chunk.ChunkIndex,
                Score: Math.Round(1 - chunk.Distance, 4)))
            .ToList();

        var answer = await chatCompletionService.CompleteAsync(
            [
                new ChatMessageInput("system", BuildSystemPrompt()),
                new ChatMessageInput("user", BuildUserPrompt(model.Question, sources))
            ],
            cancellationToken);

        await SaveConversationAsync(session, model.Question, answer, sources, cancellationToken);

        return Results.Ok(new AskQuestionResponse(session.Id, answer, sources));
    }

    private async Task<ChatSession> GetOrCreateSessionAsync(AskQuestionModel model, CancellationToken cancellationToken)
    {
        if (model.SessionId.HasValue)
        {
            var existingSession = await dbContext.ChatSessions
                .FirstOrDefaultAsync(session => session.Id == model.SessionId.Value, cancellationToken);

            if (existingSession is not null)
            {
                return existingSession;
            }
        }

        var now = DateTimeOffset.UtcNow;
        var session = new ChatSession
        {
            Id = Guid.NewGuid(),
            Title = BuildSessionTitle(model.Question),
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.ChatSessions.Add(session);
        await dbContext.SaveChangesAsync(cancellationToken);

        return session;
    }

    private async Task SaveConversationAsync(
        ChatSession session,
        string question,
        string answer,
        IReadOnlyList<AskQuestionSource> sources,
        CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        dbContext.ChatMessages.Add(new ChatMessage
        {
            Id = Guid.NewGuid(),
            ChatSessionId = session.Id,
            Role = "user",
            Content = question,
            CreatedAt = now
        });

        dbContext.ChatMessages.Add(new ChatMessage
        {
            Id = Guid.NewGuid(),
            ChatSessionId = session.Id,
            Role = "assistant",
            Content = answer,
            SourcesJson = JsonSerializer.Serialize(sources),
            CreatedAt = now
        });

        session.Title ??= BuildSessionTitle(question);
        session.UpdatedAt = now;

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string BuildSessionTitle(string question)
    {
        var trimmedQuestion = question.Trim();
        return trimmedQuestion.Length <= 80
            ? trimmedQuestion
            : $"{trimmedQuestion[..77]}...";
    }

    private static string BuildSystemPrompt()
    {
        return """
            Sen şirket içi dokümanlar üzerinden cevap veren bir RAG asistanısın.
            Cevabını Türkçe ver.
            Sadece kullanıcı mesajında verilen kaynaklara dayan.
            Kaynaklarda cevap yoksa bunu açıkça söyle.
            Tahmin yapma, dış bilgi ekleme.
            Cevabı kısa, net ve iş odaklı tut.
            """;
    }

    private static string BuildUserPrompt(string question, IReadOnlyList<AskQuestionSource> sources)
    {
        var builder = new StringBuilder();

        builder.AppendLine("Soru:");
        builder.AppendLine(question);
        builder.AppendLine();
        builder.AppendLine("Kaynaklar:");

        for (var index = 0; index < sources.Count; index++)
        {
            var source = sources[index];
            builder.AppendLine($"[{index + 1}] Doküman: {source.DocumentName}");
            builder.AppendLine($"Sayfa: {source.PageNumber?.ToString() ?? "Yok"}");
            builder.AppendLine($"Chunk: {source.ChunkIndex}");
            builder.AppendLine("İçerik:");
            builder.AppendLine(source.Content);
            builder.AppendLine();
        }

        builder.AppendLine("Yanıt:");

        return builder.ToString();
    }
}
