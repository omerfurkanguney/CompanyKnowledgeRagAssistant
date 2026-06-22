namespace CompanyKnowledgeApi.Features.Search.SemanticSearch;

public sealed record SemanticSearchResult(
    Guid DocumentId,
    string DocumentName,
    Guid ChunkId,
    string Content,
    int? PageNumber,
    int ChunkIndex,
    double Score);
