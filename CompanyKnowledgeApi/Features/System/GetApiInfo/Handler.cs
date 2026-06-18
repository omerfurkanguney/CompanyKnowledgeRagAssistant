namespace CompanyKnowledgeApi.Features.System.GetApiInfo;

public static class Handler
{
    public static Response Handle(IHostEnvironment environment)
    {
        return new Response(
            Name: "Company Knowledge RAG Assistant API",
            Environment: environment.EnvironmentName,
            Version: typeof(Handler).Assembly.GetName().Version?.ToString() ?? "unknown");
    }
}
