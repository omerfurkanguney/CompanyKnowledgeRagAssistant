namespace CompanyKnowledgeApi.Infrastructure.Documents.Extraction;

public interface ITextExtractor
{
    bool CanExtract(string contentType, string fileName);

    Task<IReadOnlyList<ExtractedTextPage>> ExtractAsync(string fullPath, CancellationToken cancellationToken);
}
