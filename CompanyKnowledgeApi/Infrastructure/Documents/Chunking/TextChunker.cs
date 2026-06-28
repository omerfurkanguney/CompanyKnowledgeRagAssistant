using System.Text;
using System.Text.RegularExpressions;
using CompanyKnowledgeApi.Infrastructure.Documents.Cleaning;
using CompanyKnowledgeApi.Infrastructure.Documents.Extraction;
using Microsoft.Extensions.Options;

namespace CompanyKnowledgeApi.Infrastructure.Documents.Chunking;

public sealed partial class TextChunker(ITextCleaner textCleaner, IOptions<DocumentChunkingOptions> options) : ITextChunker
{
    private readonly DocumentChunkingOptions _options = options.Value;

    public IReadOnlyList<TextChunk> Chunk(IReadOnlyList<ExtractedTextPage> pages)
    {
        var segments = BuildStructuralSegments(pages);
        var chunks = new List<TextChunk>();
        var chunkIndex = 0;

        foreach (var segment in segments)
        {
            foreach (var chunkContent in SplitText(segment.Content))
            {
                chunks.Add(new TextChunk(
                    Content: chunkContent,
                    StartPageNumber: segment.StartPageNumber,
                    EndPageNumber: segment.EndPageNumber,
                    Heading: segment.Heading,
                    ClauseId: segment.ClauseId,
                    ChunkType: segment.ChunkType,
                    ChunkIndex: chunkIndex++,
                    EstimatedTokenCount: EstimateTokenCount(chunkContent)));
            }
        }

        return chunks;
    }

    private IReadOnlyList<StructuralSegment> BuildStructuralSegments(IReadOnlyList<ExtractedTextPage> pages)
    {
        var segments = new List<StructuralSegment>();
        var buffer = new StringBuilder();
        string? currentHeading = null;
        string? currentClauseId = null;
        string currentChunkType = "Page";
        int? startPageNumber = null;
        int? endPageNumber = null;

        foreach (var page in pages)
        {
            var cleanText = textCleaner.Clean(page.Text);

            if (string.IsNullOrWhiteSpace(cleanText))
            {
                continue;
            }

            foreach (var line in cleanText.Split('\n'))
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    continue;
                }

                if (TryReadClause(trimmedLine, out var clauseId, out var heading))
                {
                    FlushSegment();
                    currentClauseId = clauseId;
                    currentHeading = heading ?? currentHeading;
                    currentChunkType = "Clause";
                }
                else if (IsLikelyHeading(trimmedLine))
                {
                    FlushSegment();
                    currentHeading = trimmedLine;
                    currentClauseId = null;
                    currentChunkType = "Heading";
                }

                startPageNumber ??= page.PageNumber;
                endPageNumber = page.PageNumber ?? endPageNumber;
                buffer.AppendLine(trimmedLine);
            }
        }

        FlushSegment();
        return segments;

        void FlushSegment()
        {
            var content = buffer.ToString().Trim();

            if (!string.IsNullOrWhiteSpace(content))
            {
                segments.Add(new StructuralSegment(
                    Content: content,
                    StartPageNumber: startPageNumber,
                    EndPageNumber: endPageNumber,
                    Heading: currentHeading,
                    ClauseId: currentClauseId,
                    ChunkType: currentChunkType));
            }

            buffer.Clear();
            startPageNumber = null;
            endPageNumber = null;
        }
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
            var maxEnd = Math.Min(start + _options.MaxChunkCharacters, text.Length);
            var end = maxEnd == text.Length ? maxEnd : FindSplitBoundary(text, start, maxEnd);
            var chunk = text[start..end].Trim();

            if (!string.IsNullOrWhiteSpace(chunk))
            {
                yield return chunk;
            }

            if (end >= text.Length)
            {
                break;
            }

            start = SkipWhitespace(text, end);
        }
    }

    private static int FindSplitBoundary(string text, int start, int maxEnd)
    {
        var searchLength = maxEnd - start;
        var sentenceEnd = text.LastIndexOfAny(['.', '!', '?', ';', ':'], maxEnd - 1, searchLength);

        if (sentenceEnd > start + 200)
        {
            return sentenceEnd + 1;
        }

        var lineBreak = text.LastIndexOf('\n', maxEnd - 1, searchLength);

        if (lineBreak > start + 200)
        {
            return lineBreak;
        }

        var wordBreak = text.LastIndexOf(' ', maxEnd - 1, searchLength);

        if (wordBreak > start + 200)
        {
            return wordBreak;
        }

        return maxEnd;
    }

    private static int SkipWhitespace(string text, int start)
    {
        while (start < text.Length && char.IsWhiteSpace(text[start]))
        {
            start++;
        }

        return start;
    }

    private static bool TryReadClause(string line, out string? clauseId, out string? heading)
    {
        var match = ClauseRegex().Match(line);

        if (!match.Success)
        {
            clauseId = null;
            heading = null;
            return false;
        }

        clauseId = Regex.Replace(match.Groups["id"].Value, @"\s+", string.Empty);
        heading = match.Groups["title"].Value.Trim();
        heading = string.IsNullOrWhiteSpace(heading) ? null : heading;
        return true;
    }

    private static bool IsLikelyHeading(string line)
    {
        if (line.Length is < 4 or > 140)
        {
            return false;
        }

        if (line.EndsWith('.'))
        {
            return false;
        }

        var letterCount = line.Count(char.IsLetter);
        if (letterCount < 3)
        {
            return false;
        }

        var upperCount = line.Count(char.IsUpper);
        var upperRatio = upperCount / (double)letterCount;

        return upperRatio >= 0.65 || HeadingKeywordRegex().IsMatch(line);
    }

    private static int EstimateTokenCount(string text)
    {
        return Math.Max(1, (int)Math.Ceiling(text.Length / 4.0));
    }

    [GeneratedRegex(@"^\s*(?:(?:Madde|ARTICLE)\s*)?(?<id>\d+(?:\s*\.\s*\d+)*)(?:\s*[\.)\-:])?\s+(?<title>[^\n]{2,180})\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex ClauseRegex();

    [GeneratedRegex(@"(Politika|Süreç|Kurallar|Kapsam|Amaç|Tanımlar|Başvuru|Onay|Güvenlik|Değerlendirme|Teslim|Bildirim|Doküman|Bilgisi|Yönetici)", RegexOptions.IgnoreCase)]
    private static partial Regex HeadingKeywordRegex();

    private sealed record StructuralSegment(
        string Content,
        int? StartPageNumber,
        int? EndPageNumber,
        string? Heading,
        string? ClauseId,
        string ChunkType);
}
