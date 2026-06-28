namespace CompanyKnowledgeApi.Features.Documents.ListDocuments;

public sealed record ListDocumentsResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeInBytes,
    int? PageCount,
    Guid? DepartmentId,
    string? DepartmentName,
    Guid? CategoryId,
    string? CategoryName,
    string Status,
    int ChunkCount,
    string? FailureReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
