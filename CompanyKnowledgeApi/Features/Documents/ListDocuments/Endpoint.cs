namespace CompanyKnowledgeApi.Features.Documents.ListDocuments;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapListDocumentsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", Handler.Handle)
            .WithName("ListDocuments")
            .WithSummary("Lists uploaded documents.")
            .Produces<IReadOnlyList<Response>>();

        return app;
    }
}
