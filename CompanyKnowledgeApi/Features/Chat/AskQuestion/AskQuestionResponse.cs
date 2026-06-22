namespace CompanyKnowledgeApi.Features.Chat.AskQuestion;

public sealed record AskQuestionResponse(
    string Answer,
    IReadOnlyList<AskQuestionSource> Sources);
