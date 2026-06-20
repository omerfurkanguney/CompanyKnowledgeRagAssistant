namespace CompanyKnowledgeApi.Database.Entities;

public sealed class ChatFeedback
{
    public Guid Id { get; set; }

    public Guid ChatMessageId { get; set; }

    public ChatMessage ChatMessage { get; set; } = null!;

    public string Rating { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
