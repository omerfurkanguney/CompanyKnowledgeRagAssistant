namespace CompanyKnowledgeApi.Features.Home.GetHomeSummary;

public sealed record DepartmentDocumentCountResponse(
    Guid? DepartmentId,
    string DepartmentName,
    int DocumentCount);
