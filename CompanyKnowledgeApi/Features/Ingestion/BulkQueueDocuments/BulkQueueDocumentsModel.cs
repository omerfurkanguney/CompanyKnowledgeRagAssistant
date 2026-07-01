namespace CompanyKnowledgeApi.Features.Ingestion.BulkQueueDocuments;

public sealed record BulkQueueDocumentsModel(
    string Action,
    IReadOnlyList<Guid>? DocumentIds = null,
    bool OnlyPending = true);
