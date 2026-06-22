namespace CompanyKnowledgeApi.Infrastructure.Ai.Embeddings;

public sealed class EmbeddingOptions
{
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";

    public string EmbeddingModel { get; set; } = "bge-m3";

    public int ExpectedDimensions { get; set; } = 1024;
}
