namespace CompanyKnowledgeApi.Features.Home.GetHomeSummary;

public sealed record GetHomeSummaryResponse(
    int TotalDocumentCount,
    int TotalUserCount,
    int TodayQuestionCount,
    IReadOnlyList<RecentDocumentResponse> RecentDocuments,
    IReadOnlyList<DepartmentDocumentCountResponse> DepartmentDocumentCounts,
    IReadOnlyList<CategoryDocumentCountResponse> CategoryDocumentCounts);
