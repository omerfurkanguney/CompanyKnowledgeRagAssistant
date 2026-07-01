using CompanyKnowledgeApi.Features.Ingestion.BulkQueueDocuments;
using CompanyKnowledgeApi.Features.Ingestion.EmbedDocument;
using CompanyKnowledgeApi.Features.Ingestion.ProcessDocument;
using CompanyKnowledgeApi.Features.Ingestion.RetryDocument;

namespace CompanyKnowledgeApi.Features.Ingestion;

public static class IngestionEndpoints
{
    public static IEndpointRouteBuilder MapIngestionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/documents")
            .WithTags("Documents");

        group.MapProcessDocumentEndpoint();
        group.MapEmbedDocumentEndpoint();
        group.MapRetryDocumentEndpoint();
        group.MapBulkQueueDocumentsEndpoint();

        return app;
    }
}
