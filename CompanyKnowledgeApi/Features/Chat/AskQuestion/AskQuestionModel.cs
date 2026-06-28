namespace CompanyKnowledgeApi.Features.Chat.AskQuestion;

public sealed record AskQuestionModel(string Question, int? TopK = null, Guid? SessionId = null);
