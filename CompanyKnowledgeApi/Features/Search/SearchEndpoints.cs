using CompanyKnowledgeApi.Features.Search.SemanticSearch;

namespace CompanyKnowledgeApi.Features.Search;

public static class SearchEndpoints
{
    public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/search")
            .WithTags("Search");

        group.MapSemanticSearchEndpoint();

        return app;
    }
}
