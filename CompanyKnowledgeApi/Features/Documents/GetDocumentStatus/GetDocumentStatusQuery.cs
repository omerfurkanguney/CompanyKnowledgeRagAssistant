using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Documents.GetDocumentStatus;

public sealed class GetDocumentStatusQuery(AppDbContext dbContext)
    : IQuery<GetDocumentStatusModel, GetDocumentStatusResponse?>, IScopedService
{
    public async Task<GetDocumentStatusResponse?> Handle(
        GetDocumentStatusModel model,
        CancellationToken cancellationToken)
    {
        return await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Id == model.Id)
            .Select(document => new GetDocumentStatusResponse(
                document.Id,
                document.FileName,
                document.Status.ToString(),
                document.FailureReason,
                document.CreatedAt,
                document.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
