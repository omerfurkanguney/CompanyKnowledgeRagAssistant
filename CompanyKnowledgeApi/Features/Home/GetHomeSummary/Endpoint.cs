namespace CompanyKnowledgeApi.Features.Home.GetHomeSummary;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapGetHomeSummaryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/summary", async (
                GetHomeSummaryQuery query,
                CancellationToken cancellationToken) =>
            Results.Ok(await query.Handle(cancellationToken)))
            .WithName("GetHomeSummary")
            .WithSummary("Returns dashboard summary data for the home page.")
            .Produces<GetHomeSummaryResponse>();

        return app;
    }
}
