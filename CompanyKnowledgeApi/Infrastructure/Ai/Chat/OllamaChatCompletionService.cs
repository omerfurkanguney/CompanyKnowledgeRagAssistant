using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace CompanyKnowledgeApi.Infrastructure.Ai.Chat;

public sealed class OllamaChatCompletionService(HttpClient httpClient, IOptions<ChatOptions> options)
    : IChatCompletionService
{
    private readonly ChatOptions _options = options.Value;

    public async Task<string> CompleteAsync(
        IReadOnlyList<ChatMessageInput> messages,
        CancellationToken cancellationToken)
    {
        if (messages.Count == 0)
        {
            throw new ArgumentException("At least one chat message is required.", nameof(messages));
        }

        var request = new OllamaChatRequest(
            Model: _options.ChatModel,
            Messages: messages.Select(message => new OllamaChatMessage(message.Role, message.Content)).ToList(),
            Stream: false,
            Options: new OllamaChatRequestOptions(_options.Temperature));

        var response = await httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Ollama chat request failed. StatusCode: {(int)response.StatusCode} ({response.StatusCode}). " +
                $"BaseUrl: {httpClient.BaseAddress}. Model: {_options.ChatModel}. Response: {error}",
                inner: null,
                statusCode: response.StatusCode);
        }

        var result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken);
        var content = result?.Message?.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("Ollama did not return a chat response.");
        }

        return content.Trim();
    }

    private sealed record OllamaChatRequest(
        [property: JsonPropertyName("model")] string Model,
        [property: JsonPropertyName("messages")] IReadOnlyList<OllamaChatMessage> Messages,
        [property: JsonPropertyName("stream")] bool Stream,
        [property: JsonPropertyName("options")] OllamaChatRequestOptions Options);

    private sealed record OllamaChatRequestOptions(
        [property: JsonPropertyName("temperature")] double Temperature);

    private sealed record OllamaChatMessage(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("content")] string Content);

    private sealed record OllamaChatResponse(
        [property: JsonPropertyName("message")] OllamaChatMessage? Message);
}
