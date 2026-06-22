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
            .WithSummary("Generates embeddings for document chunks using Ollama.")
            .Produces<EmbedDocumentResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
