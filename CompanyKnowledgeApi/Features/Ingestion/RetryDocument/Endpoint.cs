using CompanyKnowledgeApi.Features.Ingestion;

namespace CompanyKnowledgeApi.Features.Ingestion.RetryDocument;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapRetryDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/retry", (
                Guid id,
                RetryDocumentCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(new RetryDocumentModel(id), cancellationToken))
            .WithName("RetryDocumentIngestion")
            .WithSummary("Queues a failed document for processing or embedding retry.")
            .Produces<QueueDocumentJobResponse>(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
