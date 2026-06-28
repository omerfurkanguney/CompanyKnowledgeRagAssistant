using CompanyKnowledgeApi.Features.Chat.AskQuestion;

namespace CompanyKnowledgeApi.Features.Chat.GetChatSession;

public sealed record ChatSessionDetailResponse(
    Guid Id,
    string Title,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<ChatMessageResponse> Messages);

public sealed record ChatMessageResponse(
    Guid Id,
    string Role,
    string Content,
    IReadOnlyList<AskQuestionSource> Sources,
    DateTimeOffset CreatedAt);
