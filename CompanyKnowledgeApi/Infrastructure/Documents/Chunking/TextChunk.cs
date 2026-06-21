namespace CompanyKnowledgeApi.Infrastructure.Documents.Chunking;

public sealed record TextChunk(string Content, int? PageNumber, int ChunkIndex, int EstimatedTokenCount);
