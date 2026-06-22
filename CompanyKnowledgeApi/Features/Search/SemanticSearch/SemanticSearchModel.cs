namespace CompanyKnowledgeApi.Features.Search.SemanticSearch;

public sealed record SemanticSearchModel(
    string Question,
    int? TopK = null);
