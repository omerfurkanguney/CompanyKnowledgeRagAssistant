using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Lookups.ListDepartments;

public sealed class ListDepartmentsQuery(AppDbContext dbContext)
    : IQuery<IReadOnlyList<DepartmentLookupResponse>>, IScopedService
{
    public async Task<IReadOnlyList<DepartmentLookupResponse>> Handle(CancellationToken cancellationToken)
    {
        return await dbContext.Departments
            .AsNoTracking()
            .OrderBy(department => department.Name)
            .Select(department => new DepartmentLookupResponse(
                department.Id,
                department.Name,
                department.Slug))
            .ToListAsync(cancellationToken);
    }
}
