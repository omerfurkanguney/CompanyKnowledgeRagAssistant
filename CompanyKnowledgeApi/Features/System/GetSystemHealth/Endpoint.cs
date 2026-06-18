namespace CompanyKnowledgeApi.Features.System.GetSystemHealth;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetSystemHealthEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", Handler.Handle)
            .WithName("GetSystemHealth")
            .WithSummary("Returns API health status.");

        return app;
    }
}
