using FluentValidation;

namespace HireFlow.Application.Features.JobApplications.Commands.CreateApplication;

public class CreateApplicationValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.JobId)
            .NotEmpty().WithMessage("Job id is required.");

        RuleFor(x => x.CoverLetter)
            .MaximumLength(2000).When(x => x.CoverLetter is not null)
            .WithMessage("Cover letter cannot exceed 2000 characters.");
    }
}