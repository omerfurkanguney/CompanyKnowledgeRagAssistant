using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.BackgroundJobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Ingestion.BulkQueueDocuments;

public sealed class BulkQueueDocumentsCommand(AppDbContext dbContext, IBackgroundJobClient backgroundJobClient)
    : ICommand<BulkQueueDocumentsModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(BulkQueueDocumentsModel model, CancellationToken cancellationToken)
    {
        var action = model.Action.Trim().ToLowerInvariant();
        if (action is not ("process" or "embed" or "retry"))
        {
            return Results.BadRequest("Action must be one of: process, embed, retry.");
        }

        var query = dbContext.Documents
            .Include(document => document.Chunks)
            .Where(document => document.Status != DocumentStatus.Deleted);

        if (model.DocumentIds is { Count: > 0 })
        {
            query = query.Where(document => model.DocumentIds.Contains(document.Id));
        }

        query = action switch
        {
            "process" => query.Where(document =>
                model.OnlyPending
                    ? document.Status == DocumentStatus.Uploaded
                    : document.Status == DocumentStatus.Uploaded || document.Status == DocumentStatus.Failed),
            "embed" => query.Where(document =>
                document.Chunks.Count > 0 &&
                (model.OnlyPending
                    ? document.Status == DocumentStatus.Processed
                    : document.Status == DocumentStatus.Processed || document.Status == DocumentStatus.Failed)),
            "retry" => query.Where(document => document.Status == DocumentStatus.Failed),
            _ => query
        };

        var documents = await query.ToListAsync(cancellationToken);
        var queuedDocumentIds = new List<Guid>();

        foreach (var document in documents)
        {
            switch (action)
            {
                case "process":
                    document.Status = DocumentStatus.ProcessingQueued;
                    backgroundJobClient.Enqueue<DocumentIngestionJob>(
                        job => job.ProcessAsync(document.Id, CancellationToken.None));
                    break;
                case "embed":
                    document.Status = DocumentStatus.EmbeddingQueued;
                    backgroundJobClient.Enqueue<DocumentIngestionJob>(
                        job => job.EmbedAsync(document.Id, CancellationToken.None));
                    break;
                case "retry":
                    document.Status = document.Chunks.Count > 0
                        ? DocumentStatus.EmbeddingQueued
                        : DocumentStatus.ProcessingQueued;
                    backgroundJobClient.Enqueue<DocumentIngestionJob>(
                        job => job.RetryAsync(document.Id, CancellationToken.None));
                    break;
            }

            document.FailureReason = null;
            document.UpdatedAt = DateTimeOffset.UtcNow;
            queuedDocumentIds.Add(document.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.Accepted(
            "/api/documents",
            new BulkQueueDocumentsResponse(action, queuedDocumentIds.Count, queuedDocumentIds));
    }
}
