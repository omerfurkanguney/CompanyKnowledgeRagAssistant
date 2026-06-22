using FluentValidation;

namespace CompanyKnowledgeApi.Features.Search.SemanticSearch;

public sealed class SemanticSearchValidator : AbstractValidator<SemanticSearchModel>
{
    public SemanticSearchValidator()
    {
        RuleFor(model => model.Question)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(model => model.TopK)
            .InclusiveBetween(1, 20)
            .When(model => model.TopK.HasValue);
    }
}
