namespace CompanyKnowledgeApi.Features.Documents.UploadDocument;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapUploadDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", (
                IFormFile file,
                UploadDocumentCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(new UploadDocumentModel(file), cancellationToken))
            .WithName("UploadDocument")
            .WithSummary("Uploads a PDF or DOCX document.")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadDocumentResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .DisableAntiforgery();

        return app;
    }
}
