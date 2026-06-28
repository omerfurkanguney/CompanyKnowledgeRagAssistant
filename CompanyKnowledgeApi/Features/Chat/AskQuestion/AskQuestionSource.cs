namespace CompanyKnowledgeApi.Features.Chat.AskQuestion;

public sealed record AskQuestionSource(
    Guid DocumentId,
    string DocumentName,
    Guid ChunkId,
    string Content,
    int? StartPageNumber,
    int? EndPageNumber,
    string? Heading,
    string? ClauseId,
    string ChunkType,
    int ChunkIndex,
    double Score);
