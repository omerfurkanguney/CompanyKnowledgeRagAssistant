namespace CompanyKnowledgeApi.Infrastructure.BackgroundJobs;

public sealed class BackgroundJobOptions
{
    public int WorkerCount { get; set; } = 1;

    public bool EnableDashboard { get; set; }
}
