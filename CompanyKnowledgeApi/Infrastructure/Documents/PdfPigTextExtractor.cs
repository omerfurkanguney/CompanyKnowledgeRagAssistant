using UglyToad.PdfPig;

namespace CompanyKnowledgeApi.Infrastructure.Documents;

public sealed class PdfPigTextExtractor : ITextExtractor
{
    public bool CanExtract(string contentType, string fileName)
    {
        return contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase)
            || Path.GetExtension(fileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    public Task<IReadOnlyList<ExtractedTextPage>> ExtractAsync(string fullPath, CancellationToken cancellationToken)
    {
        var pages = new List<ExtractedTextPage>();

        using var document = PdfDocument.Open(fullPath);

        foreach (var page in document.GetPages())
        {
            cancellationToken.ThrowIfCancellationRequested();
            pages.Add(new ExtractedTextPage(page.Number, page.Text));
        }

        return Task.FromResult<IReadOnlyList<ExtractedTextPage>>(pages);
    }
}
