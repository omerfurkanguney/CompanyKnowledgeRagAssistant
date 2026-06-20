using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Storage;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CompanyKnowledgeApi.Features.Documents.UploadDocument;

public static class Handler
{
    public static async Task<IResult> Handle(
        [FromForm] IFormFile file,
        IValidator<Command> validator,
        IFileStorage fileStorage,
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var command = new Command(file);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var storedFile = await fileStorage.SaveAsync(file, cancellationToken);

        var document = new Document
        {
            Id = Guid.NewGuid(),
            FileName = storedFile.OriginalFileName,
            StoredFileName = storedFile.StoredFileName,
            StoragePath = storedFile.RelativePath,
            ContentType = storedFile.ContentType,
            SizeInBytes = storedFile.SizeInBytes,
            Status = DocumentStatus.Uploaded,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new Response(
            Id: document.Id,
            FileName: document.FileName,
            ContentType: document.ContentType,
            SizeInBytes: document.SizeInBytes,
            Status: document.Status.ToString(),
            CreatedAt: document.CreatedAt);

        return Results.Created($"/api/documents/{document.Id}/status", response);
    }
}
