namespace CompanyKnowledgeApi.Features.Documents.UploadDocument;

using Microsoft.AspNetCore.Mvc;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapUploadDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", (
                [FromForm] IFormFile file,
                [FromForm] Guid? departmentId,
                [FromForm] Guid? categoryId,
                UploadDocumentCommand command,
                CancellationToken cancellationToken) =>
            command.Handle(new UploadDocumentModel(file, departmentId, categoryId), cancellationToken))
            .WithName("UploadDocument")
            .WithSummary("Uploads a PDF or DOCX document.")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadDocumentResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .DisableAntiforgery();

        return app;
    }
}
