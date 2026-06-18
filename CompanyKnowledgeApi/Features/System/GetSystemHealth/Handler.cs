namespace CompanyKnowledgeApi.Features.System.GetSystemHealth;

public static class Handler
{
    public static Response Handle(IHostEnvironment environment)
    {
        return new Response(
            Status: "Healthy",
            Environment: environment.EnvironmentName,
            CheckedAtUtc: DateTimeOffset.UtcNow);
    }
}
