using CompanyKnowledgeApi.Features.Ingestion;

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
            .WithSummary("Queues text extraction and chunking for an uploaded document.")
            .Produces<QueueDocumentJobResponse>(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
