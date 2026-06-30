using Microsoft.Extensions.Options;

namespace CompanyKnowledgeApi.Infrastructure.Storage;

public sealed class LocalFileStorage(
    IWebHostEnvironment environment,
    IOptions<DocumentStorageOptions> options) : IFileStorage
{
    private readonly DocumentStorageOptions _options = options.Value;

    public async Task<StoredFile> SaveAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var rootPath = GetRootPath();
        var fullPath = Path.Combine(rootPath, storedFileName);

        Directory.CreateDirectory(rootPath);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream, cancellationToken);

        return new StoredFile(
            OriginalFileName: Path.GetFileName(file.FileName),
            StoredFileName: storedFileName,
            RelativePath: Path.Combine(_options.RootPath, storedFileName).Replace('\\', '/'),
            ContentType: file.ContentType,
            SizeInBytes: file.Length);
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(environment.ContentRootPath, relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string GetRootPath()
    {
        return Path.Combine(environment.ContentRootPath, _options.RootPath);
    }
}
