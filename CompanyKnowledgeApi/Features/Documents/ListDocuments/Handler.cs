using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Documents.ListDocuments;

public static class Handler
{
    public static async Task<IResult> Handle(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var documents = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Status != DocumentStatus.Deleted)
            .OrderByDescending(document => document.CreatedAt)
            .Select(document => new Response(
                document.Id,
                document.FileName,
                document.ContentType,
                document.SizeInBytes,
                document.Status.ToString(),
                document.CreatedAt,
                document.UpdatedAt))
            .ToListAsync(cancellationToken);

        return Results.Ok(documents);
    }
}
