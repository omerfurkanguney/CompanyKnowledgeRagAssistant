using CompanyKnowledgeApi.Features.Chat;
using CompanyKnowledgeApi.Features.Documents;
using CompanyKnowledgeApi.Features.Home;
using CompanyKnowledgeApi.Features.Ingestion;
using CompanyKnowledgeApi.Features.Lookups;
using CompanyKnowledgeApi.Features.Search;
using CompanyKnowledgeApi.Features.System;

namespace CompanyKnowledgeApi.Common.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapSystemEndpoints();
        api.MapDocumentEndpoints();
        api.MapHomeEndpoints();
        api.MapIngestionEndpoints();
        api.MapLookupsEndpoints();
        api.MapSearchEndpoints();
        api.MapChatEndpoints();

        return app;
    }
}
