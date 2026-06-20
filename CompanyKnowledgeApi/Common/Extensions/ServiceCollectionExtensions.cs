using CompanyKnowledgeApi.Common.Abstractions;
using System.Reflection;

namespace CompanyKnowledgeApi.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScopedServicesFrom(this IServiceCollection services, Assembly assembly)
    {
        var scopedServiceTypes = assembly.GetTypes()
            .Where(type => typeof(IScopedService).IsAssignableFrom(type))
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (var implementationType in scopedServiceTypes)
        {
            services.AddScoped(implementationType);
        }

        return services;
    }
}
