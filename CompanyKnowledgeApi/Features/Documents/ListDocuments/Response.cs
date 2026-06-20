namespace CompanyKnowledgeApi.Features.Documents.ListDocuments;

public sealed record Response(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
