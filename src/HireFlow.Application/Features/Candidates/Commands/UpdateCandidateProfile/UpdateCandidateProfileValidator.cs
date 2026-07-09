using FluentValidation;

namespace HireFlow.Application.Features.Candidates.Commands.UpdateCandidateProfile;

public class UpdateCandidateProfileValidator : AbstractValidator<UpdateCandidateProfileCommand>
{
    public UpdateCandidateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.LinkedInUrl)
            .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.LinkedInUrl))
            .WithMessage("LinkedIn URL is not valid.");

        RuleFor(x => x.PortfolioUrl)
            .Must(BeAValidUrl).When(x => !string.IsNullOrWhiteSpace(x.PortfolioUrl))
            .WithMessage("Portfolio URL is not valid.");
    }

    private static bool BeAValidUrl(string? url)
        => Uri.TryCreate(url, UriKind.Absolute, out _);
}
