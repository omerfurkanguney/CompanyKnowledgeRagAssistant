namespace CompanyKnowledgeApi.Database.Entities;

public sealed class Department
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Document> Documents { get; set; } = [];
}
