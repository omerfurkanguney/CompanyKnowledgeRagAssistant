using CompanyKnowledgeApi.Features.Documents.DeleteDocument;
using CompanyKnowledgeApi.Features.Documents.DownloadDocument;
using CompanyKnowledgeApi.Features.Documents.GetDocumentStatus;
using CompanyKnowledgeApi.Features.Documents.ListDocuments;
using CompanyKnowledgeApi.Features.Documents.UploadDocument;

namespace CompanyKnowledgeApi.Features.Documents;

public static class DocumentsEndpoints
{
    public static IEndpointRouteBuilder MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/documents")
            .WithTags("Documents");

        group.MapUploadDocumentEndpoint();
        group.MapListDocumentsEndpoint();
        group.MapGetDocumentStatusEndpoint();
        group.MapDownloadDocumentEndpoint();
        group.MapDeleteDocumentEndpoint();

        return app;
    }
}
