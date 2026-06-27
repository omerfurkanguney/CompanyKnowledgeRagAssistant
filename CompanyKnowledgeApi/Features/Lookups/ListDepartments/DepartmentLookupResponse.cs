namespace CompanyKnowledgeApi.Features.Lookups.ListDepartments;

public sealed record DepartmentLookupResponse(
    Guid Id,
    string Name,
    string Slug);
