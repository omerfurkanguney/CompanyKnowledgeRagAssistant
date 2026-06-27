using CompanyKnowledgeApi.Features.Home.GetHomeSummary;

namespace CompanyKnowledgeApi.Features.Home;

public static class HomeEndpoints
{
    public static IEndpointRouteBuilder MapHomeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/home")
            .WithTags("Home");

        group.MapGetHomeSummaryEndpoint();

        return app;
    }
}
