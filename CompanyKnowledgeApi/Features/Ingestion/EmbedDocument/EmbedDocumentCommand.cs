using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace CompanyKnowledgeApi.Features.Ingestion.EmbedDocument;

public sealed class EmbedDocumentCommand(AppDbContext dbContext, IEmbeddingService embeddingService)
    : ICommand<EmbedDocumentModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(EmbedDocumentModel model, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents
            .Include(document => document.Chunks.OrderBy(chunk => chunk.ChunkIndex))
            .FirstOrDefaultAsync(document => document.Id == model.Id, cancellationToken);

        if (document is null || document.Status == DocumentStatus.Deleted)
        {
            return Results.NotFound();
        }

        if (document.Chunks.Count == 0)
        {
            return Results.BadRequest("Document has no chunks. Process the document before embedding.");
        }

        document.Status = DocumentStatus.Embedding;
        document.FailureReason = null;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var chunks = document.Chunks
                .OrderBy(chunk => chunk.ChunkIndex)
                .ToList();

            var embeddings = await embeddingService.EmbedAsync(
                chunks.Select(chunk => chunk.Content).ToList(),
                cancellationToken);

            for (var index = 0; index < chunks.Count; index++)
            {
                chunks[index].Embedding = new Vector(embeddings[index]);
            }

            document.Status = DocumentStatus.Indexed;
            document.FailureReason = null;
            document.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new EmbedDocumentResponse(
                DocumentId: document.Id,
                Status: document.Status.ToString(),
                EmbeddedChunkCount: chunks.Count,
                FailureReason: null));
        }
        catch (Exception exception)
        {
            document.Status = DocumentStatus.Failed;
            document.FailureReason = exception.Message;
            document.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(CancellationToken.None);

            return Results.Ok(new EmbedDocumentResponse(
                DocumentId: document.Id,
                Status: document.Status.ToString(),
                EmbeddedChunkCount: 0,
                FailureReason: document.FailureReason));
        }
    }
}
