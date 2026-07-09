using FluentValidation;

namespace HireFlow.Application.Features.Team.Commands.InviteTeamMember;

public class InviteTeamMemberValidator : AbstractValidator<InviteTeamMemberCommand>
{
    public InviteTeamMemberValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(BeValidCompanyRole).WithMessage("Invalid role for a company team member.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
    }

    private static bool BeValidCompanyRole(string role)
        => new[] { "CompanyAdmin", "HRManager", "HiringManager" }.Contains(role);
}
