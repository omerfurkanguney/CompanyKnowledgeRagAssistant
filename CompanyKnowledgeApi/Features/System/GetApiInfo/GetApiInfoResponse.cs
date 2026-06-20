namespace CompanyKnowledgeApi.Features.System.GetApiInfo;

public sealed record GetApiInfoResponse(
    string Name,
    string Environment,
    string Version);
