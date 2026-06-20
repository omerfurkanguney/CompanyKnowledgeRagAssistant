namespace CompanyKnowledgeApi.Database.Entities;

public sealed class ChatMessage
{
    public Guid Id { get; set; }

    public Guid ChatSessionId { get; set; }

    public ChatSession ChatSession { get; set; } = null!;

    public string Role { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ChatFeedback> Feedback { get; set; } = [];
}
