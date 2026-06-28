using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Documents.ListDocuments;

public sealed class ListDocumentsQuery(AppDbContext dbContext)
    : IQuery<IReadOnlyList<ListDocumentsResponse>>, IScopedService
{
    public async Task<IReadOnlyList<ListDocumentsResponse>> Handle(CancellationToken cancellationToken)
    {
        return await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Status != DocumentStatus.Deleted)
            .OrderByDescending(document => document.CreatedAt)
            .Select(document => new ListDocumentsResponse(
                document.Id,
                document.FileName,
                document.ContentType,
                document.SizeInBytes,
                document.DepartmentId,
                document.Department == null ? null : document.Department.Name,
                document.CategoryId,
                document.Category == null ? null : document.Category.Name,
                document.Status.ToString(),
                document.Chunks.Count,
                document.FailureReason,
                document.CreatedAt,
                document.UpdatedAt))
            .ToListAsync(cancellationToken);
    }
}
