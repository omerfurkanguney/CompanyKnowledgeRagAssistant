using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Features.Ingestion;
using CompanyKnowledgeApi.Infrastructure.BackgroundJobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Ingestion.RetryDocument;

public sealed class RetryDocumentCommand(AppDbContext dbContext, IBackgroundJobClient backgroundJobClient)
    : ICommand<RetryDocumentModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(RetryDocumentModel model, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents
            .Include(document => document.Chunks)
            .FirstOrDefaultAsync(document => document.Id == model.Id, cancellationToken);

        if (document is null || document.Status == DocumentStatus.Deleted)
        {
            return Results.NotFound();
        }

        if (document.Status != DocumentStatus.Failed)
        {
            return Results.BadRequest("Only failed documents can be retried.");
        }

        document.Status = document.Chunks.Count > 0
            ? DocumentStatus.EmbeddingQueued
            : DocumentStatus.ProcessingQueued;
        document.FailureReason = null;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        var jobId = backgroundJobClient.Enqueue<DocumentIngestionJob>(
            job => job.RetryAsync(document.Id, CancellationToken.None));

        return Results.Accepted(
            $"/api/documents/{document.Id}/status",
            new QueueDocumentJobResponse(document.Id, document.Status.ToString(), jobId));
    }
}
