using CompanyKnowledgeApi.Common.Abstractions;

namespace CompanyKnowledgeApi.Features.System.GetSystemHealth;

public sealed class GetSystemHealthQuery(IHostEnvironment environment)
    : IScopedService
{
    public GetSystemHealthResponse Handle()
    {
        return new GetSystemHealthResponse(
            Status: "Healthy",
            Environment: environment.EnvironmentName,
            CheckedAtUtc: DateTimeOffset.UtcNow);
    }
}
