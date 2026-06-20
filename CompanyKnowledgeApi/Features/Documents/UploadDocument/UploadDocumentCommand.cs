using CompanyKnowledgeApi.Common.Abstractions;
using CompanyKnowledgeApi.Database;
using CompanyKnowledgeApi.Database.Entities;
using CompanyKnowledgeApi.Infrastructure.Storage;
using FluentValidation;

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
