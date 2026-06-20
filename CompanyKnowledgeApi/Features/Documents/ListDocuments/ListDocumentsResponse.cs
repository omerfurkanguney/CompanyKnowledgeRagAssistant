namespace CompanyKnowledgeApi.Features.Documents.ListDocuments;

public sealed record ListDocumentsResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
