namespace CompanyKnowledgeApi.Features.Lookups.ListDocumentCategories;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapListDocumentCategoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/document-categories", async (
                ListDocumentCategoriesQuery query,
                CancellationToken cancellationToken) =>
            Results.Ok(await query.Handle(cancellationToken)))
            .WithName("ListDocumentCategories")
            .WithSummary("Lists document categories.")
            .Produces<IReadOnlyList<DocumentCategoryLookupResponse>>();

        return app;
    }
}
