namespace CompanyKnowledgeApi.Database.Entities;

public enum DocumentStatus
{
    Uploaded,
    Processing,
    Processed,
    Embedding,
    Indexed,
    Failed,
    Deleted
}
