using FluentValidation;

namespace HireFlow.Application.Features.Jobs.Commands.CreateJob;

public class CreateJobValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Job title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Job type is required.")
            .Must(BeValidJobType).WithMessage("Invalid job type.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Job description is required.");

        RuleFor(x => x.SalaryMin)
            .GreaterThan(0).When(x => x.SalaryMin.HasValue)
            .WithMessage("Minimum salary must be greater than 0.");

        RuleFor(x => x.SalaryMax)
            .GreaterThan(x => x.SalaryMin).When(x => x.SalaryMax.HasValue && x.SalaryMin.HasValue)
            .WithMessage("Maximum salary must be greater than minimum salary.");

        RuleFor(x => x.ClosingDate)
            .GreaterThan(DateTime.UtcNow).When(x => x.ClosingDate.HasValue)
            .WithMessage("Closing date must be in the future.");
    }

    private static bool BeValidJobType(string type)
        => new[] { "FullTime", "PartTime", "Contract", "Remote" }.Contains(type);
}