namespace CompanyKnowledgeApi.Infrastructure.Storage;

public sealed class DocumentStorageOptions
{
    public string RootPath { get; set; } = "storage/documents";

    public long MaxFileSizeInBytes { get; set; } = 25 * 1024 * 1024;

    public string[] AllowedExtensions { get; set; } = [".pdf", ".docx"];

    public string[] AllowedContentTypes { get; set; } =
    [
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    ];
}
