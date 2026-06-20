namespace CompanyKnowledgeApi.Features.Documents.GetDocumentStatus;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetDocumentStatusEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}/status", Handler.Handle)
            .WithName("GetDocumentStatus")
            .WithSummary("Returns document processing status.")
            .Produces<Response>()
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}
