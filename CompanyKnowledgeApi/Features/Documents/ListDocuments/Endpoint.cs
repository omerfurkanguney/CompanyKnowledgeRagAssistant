namespace CompanyKnowledgeApi.Features.Documents.ListDocuments;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapListDocumentsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (
                ListDocumentsQuery query,
                CancellationToken cancellationToken) =>
            Results.Ok(await query.Handle(cancellationToken)))
            .WithName("ListDocuments")
            .WithSummary("Lists uploaded documents.")
            .Produces<IReadOnlyList<ListDocumentsResponse>>();

        return app;
    }
}
