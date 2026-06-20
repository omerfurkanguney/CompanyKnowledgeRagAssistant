using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Documents;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Ingestion.ProcessDocument;

public sealed class ProcessDocumentCommand(
    AppDbContext dbContext,
    IWebHostEnvironment environment,
    IEnumerable<ITextExtractor> textExtractors,
    ITextChunker textChunker)
    : ICommand<ProcessDocumentModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(ProcessDocumentModel model, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents
            .Include(document => document.Chunks)
            .FirstOrDefaultAsync(document => document.Id == model.Id, cancellationToken);

        if (document is null || document.Status == DocumentStatus.Deleted)
        {
            return Results.NotFound();
        }

        if (document.Status == DocumentStatus.Processing)
        {
            return Results.BadRequest("Document is already processing.");
        }

        var extractor = textExtractors.FirstOrDefault(extractor =>
            extractor.CanExtract(document.ContentType, document.FileName));

        if (extractor is null)
        {
            return Results.BadRequest("Document file type is not supported for text extraction.");
        }

        document.Status = DocumentStatus.Processing;
        document.FailureReason = null;
        document.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var fullPath = Path.Combine(environment.ContentRootPath, document.StoragePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Stored document file could not be found.", fullPath);
            }

            var pages = await extractor.ExtractAsync(fullPath, cancellationToken);
            var chunks = textChunker.Chunk(pages);

            dbContext.DocumentChunks.RemoveRange(document.Chunks);

            foreach (var chunk in chunks)
            {
                dbContext.DocumentChunks.Add(new DocumentChunk
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    Content = chunk.Content,
                    PageNumber = chunk.PageNumber,
                    ChunkIndex = chunk.ChunkIndex,
                    TokenCount = chunk.EstimatedTokenCount,
                    Embedding = null,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            document.Status = chunks.Count > 0 ? DocumentStatus.Indexed : DocumentStatus.Failed;
            document.FailureReason = chunks.Count > 0 ? null : "No extractable text was found in the document.";
            document.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            return Results.Ok(new ProcessDocumentResponse(
                DocumentId: document.Id,
                Status: document.Status.ToString(),
                ChunkCount: chunks.Count,
                FailureReason: document.FailureReason));
        }
        catch (Exception exception)
        {
            document.Status = DocumentStatus.Failed;
            document.FailureReason = exception.Message;
            document.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(CancellationToken.None);

            return Results.Ok(new ProcessDocumentResponse(
                DocumentId: document.Id,
                Status: document.Status.ToString(),
                ChunkCount: 0,
                FailureReason: document.FailureReason));
        }
    }
}
