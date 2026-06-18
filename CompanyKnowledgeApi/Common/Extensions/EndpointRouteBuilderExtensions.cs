using CompanyKnowledgeApi.Features.System;

namespace CompanyKnowledgeApi.Common.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapSystemEndpoints();

        return app;
    }
}
