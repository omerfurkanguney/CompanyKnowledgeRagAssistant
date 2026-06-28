namespace CompanyKnowledgeApi.Infrastructure.Documents.Chunking;

public sealed record TextChunk(
    string Content,
    int? StartPageNumber,
    int? EndPageNumber,
    string? Heading,
    string? ClauseId,
    string ChunkType,
    int ChunkIndex,
    int EstimatedTokenCount);
