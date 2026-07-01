using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;
using CompanyKnowledgeApi.Infrastructure.Documents.Chunking;
using CompanyKnowledgeApi.Infrastructure.Documents.Extraction;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace CompanyKnowledgeApi.Infrastructure.BackgroundJobs;

public sealed class DocumentIngestionJob(
    AppDbContext dbContext,
    IWebHostEnvironment environment,
    IEnumerable<ITextExtractor> textExtractors,
    ITextChunker textChunker,
    IEmbeddingService embeddingService)
{
    public async Task ProcessAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.Documents
            .Include(document => document.Chunks)
            .FirstOrDefaultAsync(document => document.Id == documentId, cancellationToken);

        if (document is null || document.Status == DocumentStatus.Deleted)
        {
            return;
        }

        var extractor = textExtractors.FirstOrDefault(extractor =>
            extractor.CanExtract(document.ContentType, document.FileName));

        if (extractor is null)
        {
            await MarkFailedAsync(document, "Document file type is not supported for text extraction.", cancellationToken);
            return;
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
            var pageNumbers = pages
                .Select(page => page.PageNumber)
                .Where(pageNumber => pageNumber.HasValue)
                .Select(pageNumber => pageNumber!.Value)
                .ToList();

            dbContext.DocumentChunks.RemoveRange(document.Chunks);

            foreach (var chunk in chunks)
            {
                dbContext.DocumentChunks.Add(new DocumentChunk
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    Content = chunk.Content,
                    StartPageNumber = chunk.StartPageNumber,
                    EndPageNumber = chunk.EndPageNumber,
                    Heading = chunk.Heading,
                    ClauseId = chunk.ClauseId,
                    ChunkType = chunk.ChunkType,
                    ChunkIndex = chunk.ChunkIndex,
                    TokenCount = chunk.EstimatedTokenCount,
                    Embedding = null,
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            document.Status = chunks.Count > 0 ? DocumentStatus.Processed : DocumentStatus.Failed;
            document.PageCount = pageNumbers.Count > 0 ? pageNumbers.Max() : null;
            document.FailureReason = chunks.Count > 0 ? null : "No extractable text was found in the document.";
            document.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            await MarkFailedAsync(document, exception.Message, CancellationToken.None);
        }
    }

    public async Task EmbedAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.Documents
            .Include(document => document.Chunks.OrderBy(chunk => chunk.ChunkIndex))
            .FirstOrDefaultAsync(document => document.Id == documentId, cancellationToken);

        if (document is null || document.Status == DocumentStatus.Deleted)
        {
            return;
        }

        if (document.Chunks.Count == 0)
        {
            await MarkFailedAsync(document, "Document has no chunks. Process the document before embedding.", cancellationToken);
            return;
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
        }
        catch (Exception exception)
        {
            await MarkFailedAsync(document, exception.Message, CancellationToken.None);
        }
    }

    public async Task RetryAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await dbContext.Documents
            .Include(document => document.Chunks)
            .FirstOrDefaultAsync(document => document.Id == documentId, cancellationToken);

        if (document is null || document.Status == DocumentStatus.Deleted)
        {
            return;
        }

        if (document.Chunks.Count > 0)
        {
            await EmbedAsync(documentId, cancellationToken);
            return;
        }

        await ProcessAsync(documentId, cancellationToken);
    }

    private async Task MarkFailedAsync(Document document, string failureReason, CancellationToken cancellationToken)
    {
        document.Status = DocumentStatus.Failed;
        document.FailureReason = failureReason;
        document.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
