namespace CompanyKnowledgeApi.Features.Documents.GetDocumentStatus;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetDocumentStatusEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/status", async (
                Guid id,
                GetDocumentStatusQuery query,
                CancellationToken cancellationToken) =>
            {
                var document = await query.Handle(new GetDocumentStatusModel(id), cancellationToken);

                return document is null
                    ? Results.NotFound()
                    : Results.Ok(document);
            })
            .WithName("GetDocumentStatus")
            .WithSummary("Returns document processing status.")
            .Produces<GetDocumentStatusResponse>()
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
