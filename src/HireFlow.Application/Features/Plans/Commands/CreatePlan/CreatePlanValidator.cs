using FluentValidation;

namespace HireFlow.Application.Features.Plans.Commands.CreatePlan;

public class CreatePlanValidator : AbstractValidator<CreatePlanCommand>
{
    public CreatePlanValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Plan name is required.")
            .MaximumLength(50).WithMessage("Plan name cannot exceed 50 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        RuleFor(x => x.MaxJobs)
            .GreaterThanOrEqualTo(-1).WithMessage("Use -1 for unlimited, or a non-negative number.");

        RuleFor(x => x.MaxUsers)
            .GreaterThanOrEqualTo(-1).WithMessage("Use -1 for unlimited, or a non-negative number.");

        RuleFor(x => x.MaxCandidates)
            .GreaterThanOrEqualTo(-1).WithMessage("Use -1 for unlimited, or a non-negative number.");
    }
}
