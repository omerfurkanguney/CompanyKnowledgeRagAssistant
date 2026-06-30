namespace CompanyKnowledgeApi.Infrastructure.Storage;

public sealed record StoredFile(
    string OriginalFileName,
    string StoredFileName,
    string RelativePath,
    string ContentType,
    long SizeInBytes);
