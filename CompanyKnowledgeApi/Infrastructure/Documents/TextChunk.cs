namespace CompanyKnowledgeApi.Infrastructure.Documents;

public sealed record TextChunk(string Content, int? PageNumber, int ChunkIndex, int EstimatedTokenCount);
