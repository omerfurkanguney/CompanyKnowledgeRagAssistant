using CompanyKnowledgeApi.Infrastructure.Documents.Extraction;

namespace CompanyKnowledgeApi.Infrastructure.Documents.Chunking;

public interface ITextChunker
{
    IReadOnlyList<TextChunk> Chunk(IReadOnlyList<ExtractedTextPage> pages);
}
