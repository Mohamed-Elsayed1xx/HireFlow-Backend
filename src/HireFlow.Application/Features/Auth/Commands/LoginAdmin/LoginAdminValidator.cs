using FluentValidation;

namespace HireFlow.Application.Features.Auth.Commands.LoginAdmin;

public class LoginAdminValidator : AbstractValidator<LoginAdminCommand>
{
    public LoginAdminValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}