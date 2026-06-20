namespace CompanyKnowledgeApi.Features.Documents.UploadDocument;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapUploadDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", Handler.Handle)
            .WithName("UploadDocument")
            .WithSummary("Uploads a PDF or DOCX document.")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<Response>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .DisableAntiforgery();

        return app;
    }
}
