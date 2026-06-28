using Pgvector;

namespace CompanyKnowledgeApi.Database.Entities;

public sealed class DocumentChunk
{
    public Guid Id { get; set; }

    public Guid DocumentId { get; set; }

    public Document Document { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public int? StartPageNumber { get; set; }

    public int? EndPageNumber { get; set; }

    public string? Heading { get; set; }

    public string? ClauseId { get; set; }

    public string ChunkType { get; set; } = "Fixed";

    public int ChunkIndex { get; set; }

    public int TokenCount { get; set; }

    public Vector? Embedding { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
