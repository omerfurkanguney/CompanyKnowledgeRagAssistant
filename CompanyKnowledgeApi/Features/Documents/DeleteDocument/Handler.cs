using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Storage;

namespace CompanyKnowledgeApi.Features.Documents.DeleteDocument;

public static class Handler
{
    public static async Task<IResult> Handle(
        Guid id,
        AppDbContext dbContext,
        IFileStorage fileStorage,
        CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents.FindAsync([id], cancellationToken);

        if (document is null || document.Status == DocumentStatus.Deleted)
        {
            return Results.NotFound();
        }

        await fileStorage.DeleteAsync(document.StoragePath, cancellationToken);

        document.Status = DocumentStatus.Deleted;
        document.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
