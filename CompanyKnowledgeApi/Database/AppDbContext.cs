using CompanyKnowledgeApi.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Database;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Document> Documents => Set<Document>();

    public DbSet<Department> Departments => Set<Department>();

    public DbSet<DocumentCategory> DocumentCategories => Set<DocumentCategory>();

    public DbSet<DocumentChunk> DocumentChunks => Set<DocumentChunk>();

    public DbSet<ChatSession> ChatSessions => Set<ChatSession>();

    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    public DbSet<ChatFeedback> ChatFeedback => Set<ChatFeedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Email).HasMaxLength(320).IsRequired();
            entity.Property(user => user.DisplayName).HasMaxLength(200).IsRequired();
            entity.HasIndex(user => user.Email).IsUnique();
            entity.HasMany(user => user.ChatSessions)
                .WithOne(session => session.User)
                .HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(role => role.Id);
            entity.Property(role => role.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(role => role.Name).IsUnique();
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("documents");
            entity.HasKey(document => document.Id);
            entity.Property(document => document.FileName).HasMaxLength(260).IsRequired();
            entity.Property(document => document.StoredFileName).HasMaxLength(300).IsRequired();
            entity.Property(document => document.StoragePath).HasMaxLength(1000).IsRequired();
            entity.Property(document => document.ContentType).HasMaxLength(120).IsRequired();
            entity.Property(document => document.Status).HasConversion<string>().HasMaxLength(40).IsRequired();
            entity.Property(document => document.FailureReason).HasMaxLength(1000);
            entity.HasOne(document => document.Department)
                .WithMany(department => department.Documents)
                .HasForeignKey(document => document.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(document => document.Category)
                .WithMany(category => category.Documents)
                .HasForeignKey(document => document.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(document => document.Chunks)
                .WithOne(chunk => chunk.Document)
                .HasForeignKey(chunk => chunk.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("departments");
            entity.HasKey(department => department.Id);
            entity.Property(department => department.Name).HasMaxLength(160).IsRequired();
            entity.Property(department => department.Slug).HasMaxLength(160).IsRequired();
            entity.HasIndex(department => department.Slug).IsUnique();
            entity.HasData(
                new Department { Id = new Guid("11111111-1111-1111-1111-111111111111"), Name = "İnsan Kaynakları", Slug = "human-resources", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new Department { Id = new Guid("22222222-2222-2222-2222-222222222222"), Name = "Finans", Slug = "finance", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new Department { Id = new Guid("33333333-3333-3333-3333-333333333333"), Name = "Bilgi Teknolojileri", Slug = "information-technology", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new Department { Id = new Guid("44444444-4444-4444-4444-444444444444"), Name = "Operasyon", Slug = "operations", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new Department { Id = new Guid("55555555-5555-5555-5555-555555555555"), Name = "Hukuk", Slug = "legal", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new Department { Id = new Guid("66666666-6666-6666-6666-666666666666"), Name = "Genel", Slug = "general", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) });
        });

        modelBuilder.Entity<DocumentCategory>(entity =>
        {
            entity.ToTable("document_categories");
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name).HasMaxLength(160).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(160).IsRequired();
            entity.HasIndex(category => category.Slug).IsUnique();
            entity.HasData(
                new DocumentCategory { Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Politika", Slug = "policy", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new DocumentCategory { Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Prosedür", Slug = "procedure", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new DocumentCategory { Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Kılavuz", Slug = "guide", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new DocumentCategory { Id = new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), Name = "Sözleşme", Slug = "contract", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new DocumentCategory { Id = new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Name = "CV", Slug = "cv", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                new DocumentCategory { Id = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), Name = "Diğer", Slug = "other", CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) });
        });

        modelBuilder.Entity<DocumentChunk>(entity =>
        {
            entity.ToTable("document_chunks");
            entity.HasKey(chunk => chunk.Id);
            entity.Property(chunk => chunk.Content).IsRequired();
            entity.Property(chunk => chunk.Embedding).HasColumnType("vector(1024)");
            entity.HasIndex(chunk => chunk.Embedding)
                .HasMethod("hnsw")
                .HasOperators("vector_cosine_ops");
        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.ToTable("chat_sessions");
            entity.HasKey(session => session.Id);
            entity.Property(session => session.Title).HasMaxLength(200);
            entity.HasMany(session => session.Messages)
                .WithOne(message => message.ChatSession)
                .HasForeignKey(message => message.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.ToTable("chat_messages");
            entity.HasKey(message => message.Id);
            entity.Property(message => message.Role).HasMaxLength(40).IsRequired();
            entity.Property(message => message.Content).IsRequired();
            entity.HasMany(message => message.Feedback)
                .WithOne(feedback => feedback.ChatMessage)
                .HasForeignKey(feedback => feedback.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ChatFeedback>(entity =>
        {
            entity.ToTable("chat_feedback");
            entity.HasKey(feedback => feedback.Id);
            entity.Property(feedback => feedback.Rating).HasMaxLength(40).IsRequired();
            entity.Property(feedback => feedback.Comment).HasMaxLength(1000);
        });
    }
}
