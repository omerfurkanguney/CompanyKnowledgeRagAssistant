namespace CompanyKnowledgeApi.Features.System.GetSystemHealth;

public sealed record Response(
    string Status,
    string Environment,
    DateTimeOffset CheckedAtUtc);
