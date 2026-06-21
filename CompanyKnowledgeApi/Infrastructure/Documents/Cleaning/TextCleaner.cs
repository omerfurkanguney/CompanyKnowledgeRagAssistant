using System.Text.RegularExpressions;

namespace CompanyKnowledgeApi.Infrastructure.Documents.Cleaning;

public sealed partial class TextCleaner : ITextCleaner
{
    public string Clean(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalizedLineEndings = text.Replace("\r\n", "\n").Replace('\r', '\n');
        var trimmedLines = normalizedLineEndings
            .Split('\n')
            .Select(line => WhitespaceRegex().Replace(line.Trim(), " "))
            .Where(line => !string.IsNullOrWhiteSpace(line));

        return string.Join("\n", trimmedLines).Trim();
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
