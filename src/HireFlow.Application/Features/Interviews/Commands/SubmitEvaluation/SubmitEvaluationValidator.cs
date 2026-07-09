using FluentValidation;

namespace HireFlow.Application.Features.Interviews.Commands.SubmitEvaluation;

public class SubmitEvaluationValidator : AbstractValidator<SubmitEvaluationCommand>
{
    public SubmitEvaluationValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.TechnicalScore)
            .InclusiveBetween(1, 5).When(x => x.TechnicalScore.HasValue)
            .WithMessage("Technical score must be between 1 and 5.");

        RuleFor(x => x.CultureScore)
            .InclusiveBetween(1, 5).When(x => x.CultureScore.HasValue)
            .WithMessage("Culture score must be between 1 and 5.");

        RuleFor(x => x.CommunicationScore)
            .InclusiveBetween(1, 5).When(x => x.CommunicationScore.HasValue)
            .WithMessage("Communication score must be between 1 and 5.");

        RuleFor(x => x.Recommendation)
            .NotEmpty().WithMessage("Recommendation is required.")
            .Must(BeValidRecommendation).WithMessage("Invalid recommendation value.");
    }

    private static bool BeValidRecommendation(string value)
        => new[] { "StrongYes", "Yes", "Maybe", "No", "StrongNo" }.Contains(value);
}
