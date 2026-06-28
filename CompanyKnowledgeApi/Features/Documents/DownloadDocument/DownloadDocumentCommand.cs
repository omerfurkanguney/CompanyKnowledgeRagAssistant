using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Documents.DownloadDocument;

public sealed class DownloadDocumentCommand(AppDbContext dbContext, IWebHostEnvironment environment)
    : ICommand<DownloadDocumentModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(DownloadDocumentModel model, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents
            .AsNoTracking()
            .FirstOrDefaultAsync(document => document.Id == model.Id && document.Status != DocumentStatus.Deleted, cancellationToken);

        if (document is null)
        {
            return Results.NotFound();
        }

        var fullPath = Path.GetFullPath(Path.Combine(environment.ContentRootPath, document.StoragePath));
        var contentRootPath = Path.GetFullPath(environment.ContentRootPath);

        if (!fullPath.StartsWith(contentRootPath, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
        {
            return Results.NotFound(new { message = "Stored document file could not be found." });
        }

        return Results.File(
            path: fullPath,
            contentType: document.ContentType,
            fileDownloadName: document.FileName);
    }
}
