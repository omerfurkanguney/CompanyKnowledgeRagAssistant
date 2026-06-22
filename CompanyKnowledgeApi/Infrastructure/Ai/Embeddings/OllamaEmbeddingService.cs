using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;

public sealed class OllamaEmbeddingService(HttpClient httpClient, IOptions<EmbeddingOptions> options) : IEmbeddingService
{
    private readonly EmbeddingOptions _options = options.Value;

    public async Task<IReadOnlyList<float[]>> EmbedAsync(IReadOnlyList<string> inputs, CancellationToken cancellationToken)
    {
        if (inputs.Count == 0)
        {
            return [];
        }

        var request = new OllamaEmbedRequest(_options.EmbeddingModel, inputs);
        var response = await httpClient.PostAsJsonAsync("/api/embed", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>(cancellationToken);

        if (result?.Embeddings is null)
        {
            throw new InvalidOperationException("Ollama did not return embeddings.");
        }

        if (result.Embeddings.Count != inputs.Count)
        {
            throw new InvalidOperationException("Ollama embedding count does not match input count.");
        }

        foreach (var embedding in result.Embeddings)
        {
            if (embedding.Length != _options.ExpectedDimensions)
            {
                throw new InvalidOperationException(
                    $"Embedding dimension mismatch. Expected {_options.ExpectedDimensions}, got {embedding.Length}.");
            }
        }

        return result.Embeddings;
    }

    private sealed record OllamaEmbedRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("input")] IReadOnlyList<string> Input);

    private sealed record OllamaEmbedResponse(
        [property: JsonPropertyName("embeddings")] IReadOnlyList<float[]> Embeddings);
}
