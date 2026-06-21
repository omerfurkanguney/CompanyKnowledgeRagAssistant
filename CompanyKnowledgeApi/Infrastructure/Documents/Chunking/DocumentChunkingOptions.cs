namespace CompanyKnowledgeApi.Infrastructure.Documents.Chunking;

public sealed class DocumentChunkingOptions
{
    public int MaxChunkCharacters { get; set; } = 3000;

    public int OverlapCharacters { get; set; } = 500;
}
