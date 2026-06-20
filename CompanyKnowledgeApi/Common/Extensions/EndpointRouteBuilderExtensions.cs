using CompanyKnowledgeApi.Features.Documents;
using CompanyKnowledgeApi.Features.System;

namespace CompanyKnowledgeApi.Common.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapSystemEndpoints();
        api.MapDocumentEndpoints();

        return app;
    }
}
