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

        var departmentDocumentCounts = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Status != DocumentStatus.Deleted)
            .GroupBy(document => new
            {
                document.DepartmentId,
                DepartmentName = document.Department == null ? "Departmansız" : document.Department.Name
            })
            .Select(group => new DepartmentDocumentCountResponse(
                group.Key.DepartmentId,
                group.Key.DepartmentName,
                group.Count()))
            .OrderByDescending(item => item.DocumentCount)
            .ToListAsync(cancellationToken);

        var categoryDocumentCounts = await dbContext.Documents
            .AsNoTracking()
            .Where(document => document.Status != DocumentStatus.Deleted)
            .GroupBy(document => new
            {
                document.CategoryId,
                CategoryName = document.Category == null ? "Kategorisiz" : document.Category.Name
            })
            .Select(group => new CategoryDocumentCountResponse(
                group.Key.CategoryId,
                group.Key.CategoryName,
                group.Count()))
            .OrderByDescending(item => item.DocumentCount)
            .ToListAsync(cancellationToken);

        return new GetHomeSummaryResponse(
            TotalDocumentCount: totalDocumentCount,
            TotalUserCount: totalUserCount,
            TodayQuestionCount: 0,
            RecentDocuments: recentDocuments,
            DepartmentDocumentCounts: departmentDocumentCounts,
            CategoryDocumentCounts: categoryDocumentCounts);
    }
}
