namespace CompanyKnowledgeApi.Features.System.GetApiInfo;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetApiInfoEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/info", (GetApiInfoQuery query) => query.Handle())
            .WithName("GetApiInfo")
            .WithSummary("Returns API metadata.");

        return app;
    }
}
