using CompanyKnowledgeApi.Features.System.GetApiInfo;
using CompanyKnowledgeApi.Features.System.GetSystemHealth;

namespace CompanyKnowledgeApi.Features.System;

public static class SystemEndpoints
{
    public static IEndpointRouteBuilder MapSystemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/system")
            .WithTags("System");

        group.MapGetApiInfoEndpoint();
        group.MapGetSystemHealthEndpoint();

        return app;
    }
}
