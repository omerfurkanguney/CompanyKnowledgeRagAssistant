namespace CompanyKnowledgeApi.Database.Entities;

public sealed class ChatSession
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public User? User { get; set; }

    public string? Title { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = [];
}
