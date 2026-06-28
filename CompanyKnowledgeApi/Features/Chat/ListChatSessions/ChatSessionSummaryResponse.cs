namespace CompanyKnowledgeApi.Features.Chat.ListChatSessions;

public sealed record ChatSessionSummaryResponse(
    Guid Id,
    string Title,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int MessageCount);
