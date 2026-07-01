namespace CompanyKnowledgeApi.Database.Entities;

public enum DocumentStatus
{
    Uploaded,
    ProcessingQueued,
    Processing,
    Processed,
    EmbeddingQueued,
    Embedding,
    Indexed,
    Failed,
    Deleted
}
