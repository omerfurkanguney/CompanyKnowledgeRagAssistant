namespace CompanyKnowledgeApi.Features.Documents.DeleteDocument;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapDeleteDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}", (
                Guid id,
                DeleteDocumentCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(new DeleteDocumentModel(id), cancellationToken))
            .WithName("DeleteDocument")
            .WithSummary("Marks a document as deleted and removes the local file.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
