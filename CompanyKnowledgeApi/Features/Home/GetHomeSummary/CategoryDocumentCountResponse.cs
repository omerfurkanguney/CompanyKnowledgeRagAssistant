namespace CompanyKnowledgeApi.Features.Home.GetHomeSummary;

public sealed record CategoryDocumentCountResponse(
    Guid? CategoryId,
    string CategoryName,
    int DocumentCount);
