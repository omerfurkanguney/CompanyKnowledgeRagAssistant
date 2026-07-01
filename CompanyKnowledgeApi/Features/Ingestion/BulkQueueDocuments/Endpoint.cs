namespace CompanyKnowledgeApi.Features.Ingestion.BulkQueueDocuments;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapBulkQueueDocumentsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/bulk-jobs", (
                BulkQueueDocumentsModel model,
                BulkQueueDocumentsCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(model, cancellationToken))
            .WithName("BulkQueueDocumentJobs")
            .WithSummary("Queues processing, embedding, or retry jobs for multiple documents.")
            .Produces<BulkQueueDocumentsResponse>(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
