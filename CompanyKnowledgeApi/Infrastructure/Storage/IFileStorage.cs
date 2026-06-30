namespace CompanyKnowledgeApi.Infrastructure.Storage;

public interface IFileStorage
{
    Task<StoredFile> SaveAsync(IFormFile file, CancellationToken cancellationToken);

    Task DeleteAsync(string relativePath, CancellationToken cancellationToken);
}
