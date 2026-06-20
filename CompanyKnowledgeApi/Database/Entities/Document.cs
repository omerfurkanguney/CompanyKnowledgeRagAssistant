namespace CompanyKnowledgeApi.Database.Entities;

public sealed class Document
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string StoragePath { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeInBytes { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded;

    public string? FailureReason { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<DocumentChunk> Chunks { get; set; } = [];
}
