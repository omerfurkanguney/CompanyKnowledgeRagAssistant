namespace CompanyKnowledgeApi.Features.Documents.ListDocuments;

public sealed record ListDocumentsResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeInBytes,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? CategoryId,
    string? CategoryName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
