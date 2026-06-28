using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Search.SemanticSearch;

public sealed class SemanticSearchQuery(
    AppDbContext dbContext,
    IEmbeddingService embeddingService,
    IValidator<SemanticSearchModel> validator)
    : IQuery<SemanticSearchModel, IResult>, IScopedService
{
    private const int DefaultTopK = 5;

    public async Task<IResult> Handle(SemanticSearchModel model, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var topK = model.TopK ?? DefaultTopK;
        var embeddings = await embeddingService.EmbedAsync([model.Question], cancellationToken);
        var questionEmbedding = new Vector(embeddings[0]);

        var results = await dbContext.DocumentChunks
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
                chunk.StartPageNumber,
                chunk.EndPageNumber,
                chunk.Heading,
                chunk.ClauseId,
                chunk.ChunkType,
                chunk.ChunkIndex,
                Distance = chunk.Embedding!.CosineDistance(questionEmbedding)
            })
            .ToListAsync(cancellationToken);

        var response = new SemanticSearchResponse(
            Question: model.Question,
            TopK: topK,
            Results: results
                .Select(result => new SemanticSearchResult(
                    DocumentId: result.DocumentId,
                    DocumentName: result.DocumentName,
                    ChunkId: result.ChunkId,
                    Content: result.Content,
                    StartPageNumber: result.StartPageNumber,
                    EndPageNumber: result.EndPageNumber,
                    Heading: result.Heading,
                    ClauseId: result.ClauseId,
                    ChunkType: result.ChunkType,
                    ChunkIndex: result.ChunkIndex,
                    Score: Math.Round(1 - result.Distance, 4)))
                .ToList());

        return Results.Ok(response);
    }
}
