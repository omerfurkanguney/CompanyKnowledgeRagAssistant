namespace CompanyKnowledgeApi.Features.System.GetApiInfo;

public sealed record Response(
    string Name,
    string Environment,
    string Version);
