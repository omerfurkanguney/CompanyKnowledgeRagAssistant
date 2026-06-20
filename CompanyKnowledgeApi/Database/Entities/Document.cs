namespace CompanyKnowledgeApi.Database.Entities;

public sealed class Document
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long SizeInBytes { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<DocumentChunk> Chunks { get; set; } = [];
}
