using CompanyKnowledgeApi.Features.Ingestion;

namespace CompanyKnowledgeApi.Features.Ingestion.EmbedDocument;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapEmbedDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/{id:guid}/embed", (
                Guid id,
                EmbedDocumentCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(new EmbedDocumentModel(id), cancellationToken))
            .WithName("EmbedDocument")
            .WithSummary("Queues embedding generation for document chunks using Ollama.")
            .Produces<QueueDocumentJobResponse>(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
