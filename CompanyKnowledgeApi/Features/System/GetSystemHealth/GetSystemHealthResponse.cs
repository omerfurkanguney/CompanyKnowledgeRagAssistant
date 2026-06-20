namespace CompanyKnowledgeApi.Features.System.GetSystemHealth;

public sealed record GetSystemHealthResponse(
    string Status,
    string Environment,
    DateTimeOffset CheckedAtUtc);
