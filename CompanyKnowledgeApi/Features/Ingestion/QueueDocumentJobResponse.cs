namespace CompanyKnowledgeApi.Features.Ingestion;

public sealed record QueueDocumentJobResponse(
    Guid DocumentId,
    string Status,
    string JobId);
