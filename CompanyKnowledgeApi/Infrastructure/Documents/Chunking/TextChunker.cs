using Microsoft.Extensions.Options;
using CompanyKnowledgeApi.Infrastructure.Documents.Cleaning;
using CompanyKnowledgeApi.Infrastructure.Documents.Extraction;

namespace CompanyKnowledgeApi.Infrastructure.Documents.Chunking;

public sealed class TextChunker(ITextCleaner textCleaner, IOptions<DocumentChunkingOptions> options) : ITextChunker
{
    private readonly DocumentChunkingOptions _options = options.Value;

    public IReadOnlyList<TextChunk> Chunk(IReadOnlyList<ExtractedTextPage> pages)
    {
        var chunks = new List<TextChunk>();
        var chunkIndex = 0;

        foreach (var page in pages)
        {
            var cleanText = textCleaner.Clean(page.Text);

            if (string.IsNullOrWhiteSpace(cleanText))
            {
                continue;
            }

            foreach (var chunkContent in SplitText(cleanText))
            {
                chunks.Add(new TextChunk(
                    Content: chunkContent,
                    PageNumber: page.PageNumber,
                    ChunkIndex: chunkIndex++,
                    EstimatedTokenCount: EstimateTokenCount(chunkContent)));
            }
        }

        return chunks;
    }

    private IEnumerable<string> SplitText(string text)
    {
        if (text.Length <= _options.MaxChunkCharacters)
        {
            yield return text;
            yield break;
        }

        var start = 0;

        while (start < text.Length)
        {
            var length = Math.Min(_options.MaxChunkCharacters, text.Length - start);
            var end = start + length;

            if (end < text.Length)
            {
                var lastBreak = text.LastIndexOf('\n', end - 1, length);

                if (lastBreak > start)
                {
                    end = lastBreak;
                }
            }

            var chunk = text[start..end].Trim();

            if (!string.IsNullOrWhiteSpace(chunk))
            {
                yield return chunk;
            }

            if (end >= text.Length)
            {
                break;
            }

            start = Math.Max(end - _options.OverlapCharacters, start + 1);
        }
    }

    private static int EstimateTokenCount(string text)
    {
        return Math.Max(1, (int)Math.Ceiling(text.Length / 4.0));
    }
}
