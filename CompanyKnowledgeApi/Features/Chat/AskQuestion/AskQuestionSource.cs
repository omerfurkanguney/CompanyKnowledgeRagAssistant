namespace CompanyKnowledgeApi.Features.Chat.AskQuestion;

public sealed record AskQuestionSource(
    Guid DocumentId,
    string DocumentName,
    Guid ChunkId,
    string Content,
    int? PageNumber,
    int ChunkIndex,
    double Score);
