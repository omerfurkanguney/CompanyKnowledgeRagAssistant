namespace CompanyKnowledgeApi.Features.Chat.AskQuestion;

public sealed record AskQuestionResponse(
    Guid SessionId,
    string Answer,
    IReadOnlyList<AskQuestionSource> Sources);
