namespace CompanyKnowledgeApi.Features.Ingestion.ProcessDocument;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapProcessDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/process", (
                Guid id,
                ProcessDocumentCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(new ProcessDocumentModel(id), cancellationToken))
            .WithName("ProcessDocument")
            .WithSummary("Extracts text from an uploaded document and creates chunks.")
            .Produces<ProcessDocumentResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
