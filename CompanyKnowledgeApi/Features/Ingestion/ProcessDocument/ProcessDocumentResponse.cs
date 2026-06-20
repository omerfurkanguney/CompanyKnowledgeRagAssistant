namespace CompanyKnowledgeApi.Features.Ingestion.ProcessDocument;

public sealed record ProcessDocumentResponse(
    Guid DocumentId,
    string Status,
    int ChunkCount,
    string? FailureReason);
