using FluentValidation;

namespace HireFlow.Application.Features.Jobs.Commands.UpdateJob;

public class UpdateJobValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Job id is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Job title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Job type is required.")
            .Must(t => new[] { "FullTime", "PartTime", "Contract", "Remote" }.Contains(t))
            .WithMessage("Invalid job type.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Job description is required.");

        RuleFor(x => x.SalaryMin)
            .GreaterThan(0).When(x => x.SalaryMin.HasValue)
            .WithMessage("Minimum salary must be greater than 0.");

        RuleFor(x => x.SalaryMax)
            .GreaterThan(x => x.SalaryMin).When(x => x.SalaryMax.HasValue && x.SalaryMin.HasValue)
            .WithMessage("Maximum salary must be greater than minimum salary.");
    }
}