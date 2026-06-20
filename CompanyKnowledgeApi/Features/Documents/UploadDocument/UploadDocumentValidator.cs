using CompanyKnowledgeApi.Infrastructure.Storage;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CompanyKnowledgeApi.Features.Documents.UploadDocument;

public sealed class UploadDocumentValidator : AbstractValidator<UploadDocumentModel>
{
    public UploadDocumentValidator(IOptions<DocumentStorageOptions> options)
    {
        var storageOptions = options.Value;

        RuleFor(command => command.File)
            .NotNull()
            .WithMessage("A document file is required.");

        When(command => command.File is not null, () =>
        {
            RuleFor(command => command.File!.Length)
                .GreaterThan(0)
                .WithMessage("The uploaded file is empty.")
                .LessThanOrEqualTo(storageOptions.MaxFileSizeInBytes)
                .WithMessage($"The uploaded file cannot exceed {storageOptions.MaxFileSizeInBytes} bytes.");

            RuleFor(command => command.File!.FileName)
                .Must(fileName => storageOptions.AllowedExtensions.Contains(
                    Path.GetExtension(fileName).ToLowerInvariant(),
                    StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Only these file types are allowed: {string.Join(", ", storageOptions.AllowedExtensions)}.");

            RuleFor(command => command.File!.ContentType)
                .Must(contentType => storageOptions.AllowedContentTypes.Contains(
                    contentType,
                    StringComparer.OrdinalIgnoreCase))
                .WithMessage("The uploaded file content type is not allowed.");
        });
    }
}
