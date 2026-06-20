using CompanyKnowledgeApi.Features.Ingestion.ProcessDocument;

namespace CompanyKnowledgeApi.Features.Ingestion;

public static class IngestionEndpoints
{
    public static IEndpointRouteBuilder MapIngestionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/documents")
            .WithTags("Documents");

        group.MapProcessDocumentEndpoint();

        return app;
    }
}
