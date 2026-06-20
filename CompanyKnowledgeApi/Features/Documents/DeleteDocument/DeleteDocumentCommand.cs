using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Storage;

namespace CompanyKnowledgeApi.Features.Documents.DeleteDocument;

public sealed class DeleteDocumentCommand(AppDbContext dbContext, IFileStorage fileStorage)
    : ICommand<DeleteDocumentModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(DeleteDocumentModel model, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents.FindAsync([model.Id], cancellationToken);

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
