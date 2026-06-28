using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Features.Chat.AskQuestion;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CompanyKnowledgeApi.Features.Chat.GetChatSession;

public sealed class GetChatSessionQuery(AppDbContext dbContext)
    : IQuery<Guid, IResult>, IScopedService
{
    public async Task<IResult> Handle(Guid sessionId, CancellationToken cancellationToken)
    {
        var session = await dbContext.ChatSessions
            .AsNoTracking()
            .Include(session => session.Messages.OrderBy(message => message.CreatedAt))
            .FirstOrDefaultAsync(session => session.Id == sessionId, cancellationToken);

        if (session is null)
        {
            return Results.NotFound();
        }

        var response = new ChatSessionDetailResponse(
            session.Id,
            session.Title ?? "Yeni Sohbet",
            session.CreatedAt,
            session.UpdatedAt,
            session.Messages
                .OrderBy(message => message.CreatedAt)
                .ThenBy(message => message.Role == "user" ? 0 : 1)
                .Select(message => new ChatMessageResponse(
                    message.Id,
                    message.Role,
                    message.Content,
                    DeserializeSources(message.SourcesJson),
                    message.CreatedAt))
                .ToList());

        return Results.Ok(response);
    }

    private static IReadOnlyList<AskQuestionSource> DeserializeSources(string? sourcesJson)
    {
        if (string.IsNullOrWhiteSpace(sourcesJson))
        {
            return [];
        }

        return JsonSerializer.Deserialize<IReadOnlyList<AskQuestionSource>>(sourcesJson) ?? [];
    }
}
