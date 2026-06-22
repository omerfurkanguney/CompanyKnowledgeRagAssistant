namespace CompanyKnowledgeApi.Features.Search.SemanticSearch;

public static class Endpoint
{
    public static IEndpointRouteBuilder MapSemanticSearchEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/semantic", (
                SemanticSearchModel model,
                SemanticSearchQuery query,
                CancellationToken cancellationToken) =>
            query.Handle(model, cancellationToken))
            .WithName("SemanticSearch")
            .WithSummary("Finds the most relevant document chunks for a question.")
            .Produces<SemanticSearchResponse>()
            .ProducesValidationProblem();

        return app;
    }
}
