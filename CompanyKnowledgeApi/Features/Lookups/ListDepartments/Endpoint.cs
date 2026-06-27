namespace CompanyKnowledgeApi.Features.Lookups.ListDepartments;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapListDepartmentsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/departments", async (
                ListDepartmentsQuery query,
                CancellationToken cancellationToken) =>
            Results.Ok(await query.Handle(cancellationToken)))
            .WithName("ListDepartments")
            .WithSummary("Lists document departments.")
            .Produces<IReadOnlyList<DepartmentLookupResponse>>();

        return app;
    }
}
