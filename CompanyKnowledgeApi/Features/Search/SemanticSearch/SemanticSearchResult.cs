namespace CompanyKnowledgeApi.Features.Search.SemanticSearch;

public sealed record SemanticSearchResult(
    Guid DocumentId,
    string DocumentName,
    Guid ChunkId,
    string Content,
    int? StartPageNumber,
    int? EndPageNumber,
    string? Heading,
    string? ClauseId,
    string ChunkType,
    int ChunkIndex,
    double Score);
