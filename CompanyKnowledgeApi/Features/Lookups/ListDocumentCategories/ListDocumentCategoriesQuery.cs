using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Lookups.ListDocumentCategories;

public sealed class ListDocumentCategoriesQuery(AppDbContext dbContext)
    : IQuery<IReadOnlyList<DocumentCategoryLookupResponse>>, IScopedService
{
    public async Task<IReadOnlyList<DocumentCategoryLookupResponse>> Handle(CancellationToken cancellationToken)
    {
        return await dbContext.DocumentCategories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new DocumentCategoryLookupResponse(
                category.Id,
                category.Name,
                category.Slug))
            .ToListAsync(cancellationToken);
    }
}
