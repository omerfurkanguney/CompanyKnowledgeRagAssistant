using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Features.Ingestion;
using CompanyKnowledgeApi.Infrastructure.BackgroundJobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Ingestion.EmbedDocument;

public sealed class EmbedDocumentCommand(AppDbContext dbContext, IBackgroundJobClient backgroundJobClient)
    : ICommand<EmbedDocumentModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(EmbedDocumentModel model, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents
            .Include(document => document.Chunks)
            .FirstOrDefaultAsync(document => document.Id == model.Id, cancellationToken);

        if (document is null || document.Status == DocumentStatus.Deleted)
        {
            return Results.NotFound();
        }

        if (document.Status == DocumentStatus.Embedding)
        {
            return Results.BadRequest("Document embedding is already running.");
        }

        if (document.Chunks.Count == 0)
        {
            return Results.BadRequest("Document has no chunks. Process the document before embedding.");
        }

        document.Status = DocumentStatus.EmbeddingQueued;
        document.FailureReason = null;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var jobId = backgroundJobClient.Enqueue<DocumentIngestionJob>(
            job => job.EmbedAsync(document.Id, CancellationToken.None));

        return Results.Accepted(
            $"/api/documents/{document.Id}/status",
            new QueueDocumentJobResponse(document.Id, document.Status.ToString(), jobId));
    }
}
