namespace CompanyKnowledgeApi.Features.Lookups.ListDocumentCategories;

public sealed record DocumentCategoryLookupResponse(
    Guid Id,
    string Name,
    string Slug);
