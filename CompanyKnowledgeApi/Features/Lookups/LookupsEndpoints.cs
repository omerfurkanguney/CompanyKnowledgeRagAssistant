using CompanyKnowledgeApi.Features.Lookups.ListDocumentCategories;
using CompanyKnowledgeApi.Features.Lookups.ListDepartments;

namespace CompanyKnowledgeApi.Features.Lookups;

public static class LookupsEndpoints
{
    public static IEndpointRouteBuilder MapLookupsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/lookups")
            .WithTags("Lookups");

        group.MapListDepartmentsEndpoint();
        group.MapListDocumentCategoriesEndpoint();

        return app;
    }
}
