namespace CompanyKnowledgeApi.Infrastructure.Documents;

public interface ITextChunker
{
    IReadOnlyList<TextChunk> Chunk(IReadOnlyList<ExtractedTextPage> pages);
}
