namespace CompanyKnowledgeApi.Features.Home.GetHomeSummary;

public sealed record RecentDocumentResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string Status,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? CategoryId,
    string? CategoryName,
    DateTimeOffset CreatedAt);
