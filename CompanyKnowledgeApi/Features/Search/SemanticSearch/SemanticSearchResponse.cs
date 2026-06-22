namespace CompanyKnowledgeApi.Features.Search.SemanticSearch;

public sealed record SemanticSearchResponse(
    string Question,
    int TopK,
    IReadOnlyList<SemanticSearchResult> Results);
