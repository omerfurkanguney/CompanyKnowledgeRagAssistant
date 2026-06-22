namespace CompanyKnowledgeApi.Infrastructure.Ai.Chat;

public interface IChatCompletionService
{
    Task<string> CompleteAsync(IReadOnlyList<ChatMessageInput> messages, CancellationToken cancellationToken);
}
