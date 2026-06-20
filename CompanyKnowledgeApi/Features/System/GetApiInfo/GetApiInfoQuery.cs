using CompanyKnowledgeApi.Common.Abstractions;

namespace CompanyKnowledgeApi.Features.System.GetApiInfo;

public sealed class GetApiInfoQuery(IHostEnvironment environment)
    : IScopedService
{
    public GetApiInfoResponse Handle()
    {
        return new GetApiInfoResponse(
            Name: "Company Knowledge RAG Assistant API",
            Environment: environment.EnvironmentName,
            Version: typeof(GetApiInfoQuery).Assembly.GetName().Version?.ToString() ?? "unknown");
    }
}
