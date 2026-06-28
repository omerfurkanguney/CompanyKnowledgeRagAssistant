using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Home.GetHomeSummary;

public sealed class GetHomeSummaryQuery(AppDbContext dbContext)
    : IQuery<GetHomeSummaryResponse>, IScopedService
{
    private const int RecentDocumentCount = 10;

    public async Task<GetHomeSummaryResponse> Handle(CancellationToken cancellationToken)
    {
        var totalDocumentCount = await dbContext.Documents
            .AsNoTracking()
            .CountAsync(document => document.Status != DocumentStatus.Deleted, cancellationToken);

        var totalUserCount = await dbContext.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var recentDocuments = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Status != DocumentStatus.Deleted)
            .OrderByDescending(document => document.CreatedAt)
            .Take(RecentDocumentCount)
            .Select(document => new RecentDocumentResponse(
                document.Id,
                document.FileName,
                document.ContentType,
                document.SizeInBytes,
                document.Status.ToString(),
                document.DepartmentId,
                document.Department == null ? null : document.Department.Name,
                document.CategoryId,
                document.Category == null ? null : document.Category.Name,
                document.CreatedAt))
            .ToListAsync(cancellationToken);

        var departmentNamesById = await dbContext.Departments
            .AsNoTracking()
            .ToDictionaryAsync(department => department.Id, department => department.Name, cancellationToken);

        var categoryNamesById = await dbContext.DocumentCategories
            .AsNoTracking()
            .ToDictionaryAsync(category => category.Id, category => category.Name, cancellationToken);

        var departmentCounts = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Status != DocumentStatus.Deleted)
            .GroupBy(document => document.DepartmentId)
            .Select(group => new
            {
                DepartmentId = group.Key,
                DocumentCount = group.Count()
            })
            .OrderByDescending(item => item.DocumentCount)
            .ToListAsync(cancellationToken);

        var departmentDocumentCounts = departmentCounts
            .Select(item => new DepartmentDocumentCountResponse(
                item.DepartmentId,
                item.DepartmentId.HasValue && departmentNamesById.TryGetValue(item.DepartmentId.Value, out var departmentName)
                    ? departmentName
                    : "Departmansız",
                item.DocumentCount))
            .ToList();

        var categoryCounts = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Status != DocumentStatus.Deleted)
            .GroupBy(document => document.CategoryId)
            .Select(group => new
            {
                CategoryId = group.Key,
                DocumentCount = group.Count()
            })
            .OrderByDescending(item => item.DocumentCount)
            .ToListAsync(cancellationToken);

        var categoryDocumentCounts = categoryCounts
            .Select(item => new CategoryDocumentCountResponse(
                item.CategoryId,
                item.CategoryId.HasValue && categoryNamesById.TryGetValue(item.CategoryId.Value, out var categoryName)
                    ? categoryName
                    : "Kategorisiz",
                item.DocumentCount))
            .ToList();

        return new GetHomeSummaryResponse(
            TotalDocumentCount: totalDocumentCount,
            TotalUserCount: totalUserCount,
            TodayQuestionCount: 0,
            RecentDocuments: recentDocuments,
            DepartmentDocumentCounts: departmentDocumentCounts,
            CategoryDocumentCounts: categoryDocumentCounts);
    }
}
