namespace CompanyKnowledgeApi.Features.Ingestion.BulkQueueDocuments;

public sealed record BulkQueueDocumentsResponse(
    string Action,
    int QueuedCount,
    IReadOnlyList<Guid> DocumentIds);
