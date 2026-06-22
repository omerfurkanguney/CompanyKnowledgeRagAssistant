using FluentValidation;

namespace CompanyKnowledgeApi.Features.Chat.AskQuestion;

public sealed class AskQuestionValidator : AbstractValidator<AskQuestionModel>
{
    public AskQuestionValidator()
    {
        RuleFor(model => model.Question)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(model => model.TopK)
            .InclusiveBetween(1, 10)
            .When(model => model.TopK.HasValue);
    }
}
