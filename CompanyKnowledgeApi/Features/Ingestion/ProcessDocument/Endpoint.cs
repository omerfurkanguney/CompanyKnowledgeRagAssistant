namespace CompanyKnowledgeApi.Features.Ingestion.ProcessDocument;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapProcessDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/process", Handler.Handle)
            .WithName("ProcessDocument")
            .WithSummary("Extracts text from an uploaded document and creates chunks.")
            .Produces<Response>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
