namespace CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;

public interface IEmbeddingService
{
    Task<IReadOnlyList<float[]>> EmbedAsync(IReadOnlyList<string> inputs, CancellationToken cancellationToken);
}
