using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Storage;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CompanyKnowledgeApi.Features.Documents.UploadDocument;

public sealed class UploadDocumentCommand(
    IValidator<UploadDocumentModel> validator,
    IFileStorage fileStorage,
    AppDbContext dbContext)
    : ICommand<UploadDocumentModel, IResult>, IScopedService
{
    public async Task<IResult> Handle(UploadDocumentModel model, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(model, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var file = model.File!;

        if (model.DepartmentId.HasValue &&
            !await dbContext.Departments.AnyAsync(department => department.Id == model.DepartmentId.Value, cancellationToken))
        {
            return Results.BadRequest("Selected department does not exist.");
        }

        if (model.CategoryId.HasValue &&
            !await dbContext.DocumentCategories.AnyAsync(category => category.Id == model.CategoryId.Value, cancellationToken))
        {
            return Results.BadRequest("Selected document category does not exist.");
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
            DepartmentId = model.DepartmentId,
            CategoryId = model.CategoryId,
            Status = DocumentStatus.Uploaded,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.Documents.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);

        var response = new UploadDocumentResponse(
            Id: document.Id,
            FileName: document.FileName,
            ContentType: document.ContentType,
            SizeInBytes: document.SizeInBytes,
            Status: document.Status.ToString(),
            CreatedAt: document.CreatedAt);

        return Results.Created($"/api/documents/{document.Id}/status", response);
    }
}
