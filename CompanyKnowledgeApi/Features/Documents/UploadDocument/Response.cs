namespace CompanyKnowledgeApi.Features.Documents.UploadDocument;

public sealed record Response(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string Status,
    DateTimeOffset CreatedAt);
