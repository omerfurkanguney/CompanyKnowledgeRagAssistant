namespace CompanyKnowledgeApi.Features.Ingestion.ProcessDocument;

public sealed record Response(
    Guid DocumentId,
    string Status,
    int ChunkCount,
    string? FailureReason);
