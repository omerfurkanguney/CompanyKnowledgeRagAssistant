namespace CompanyKnowledgeApi.Features.Ingestion.EmbedDocument;

public sealed record EmbedDocumentResponse(
    Guid DocumentId,
    string Status,
    int EmbeddedChunkCount,
    string? FailureReason);
