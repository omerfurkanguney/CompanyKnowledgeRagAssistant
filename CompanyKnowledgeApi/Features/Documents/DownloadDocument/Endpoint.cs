namespace CompanyKnowledgeApi.Features.Documents.DownloadDocument;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapDownloadDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/download", (
                Guid id,
                DownloadDocumentCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(new DownloadDocumentModel(id), cancellationToken))
            .WithName("DownloadDocument")
            .WithSummary("Downloads the original uploaded document file.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
