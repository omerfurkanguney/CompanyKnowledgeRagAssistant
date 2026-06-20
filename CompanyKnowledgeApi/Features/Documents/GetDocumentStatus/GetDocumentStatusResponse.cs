namespace CompanyKnowledgeApi.Features.Documents.GetDocumentStatus;

public sealed record GetDocumentStatusResponse(
    Guid Id,
    string FileName,
    string Status,
    string? FailureReason,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
