namespace CompanyKnowledgeApi.Features.Documents.UploadDocument;

public sealed record UploadDocumentResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long SizeInBytes,
    string Status,
    DateTimeOffset CreatedAt);
