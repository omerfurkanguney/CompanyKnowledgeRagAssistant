using CompanyKnowledgeApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Documents.GetDocumentStatus;

public static class Handler
{
    public static async Task<IResult> Handle(Guid id, AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var document = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Id == id)
            .Select(document => new Response(
                document.Id,
                document.FileName,
                document.Status.ToString(),
                document.FailureReason,
                document.CreatedAt,
                document.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return document is null
            ? Results.NotFound()
            : Results.Ok(document);
    }
}
