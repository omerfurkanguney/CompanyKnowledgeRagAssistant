using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CompanyKnowledgeApi.Infrastructure.Documents;

public sealed class OpenXmlDocxTextExtractor : ITextExtractor
{
    public bool CanExtract(string contentType, string fileName)
    {
        return contentType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document", StringComparison.OrdinalIgnoreCase)
            || Path.GetExtension(fileName).Equals(".docx", StringComparison.OrdinalIgnoreCase);
    }

    public Task<IReadOnlyList<ExtractedTextPage>> ExtractAsync(string fullPath, CancellationToken cancellationToken)
    {
        using var document = WordprocessingDocument.Open(fullPath, false);
        var body = document.MainDocumentPart?.Document?.Body;

        if (body is null)
        {
            return Task.FromResult<IReadOnlyList<ExtractedTextPage>>([]);
        }

        var paragraphs = body
            .Descendants<Paragraph>()
            .Select(paragraph => paragraph.InnerText)
            .Where(text => !string.IsNullOrWhiteSpace(text));

        var text = string.Join(Environment.NewLine, paragraphs);

        return Task.FromResult<IReadOnlyList<ExtractedTextPage>>(
        [
            new ExtractedTextPage(PageNumber: null, Text: text)
        ]);
    }
}
