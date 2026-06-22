namespace CompanyKnowledgeApi.Infrastructure.Ai.Chat;

public sealed class ChatOptions
{
    public string OllamaBaseUrl { get; set; } = "http://localhost:11434";
    public string ChatModel { get; set; } = "qwen3:4b";
    public double Temperature { get; set; } = 0.2;
    public int RequestTimeoutSeconds { get; set; } = 300;
    public int MaxOutputTokens { get; set; } = 256;
}
