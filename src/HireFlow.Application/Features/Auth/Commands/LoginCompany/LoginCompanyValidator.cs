using FluentValidation;

namespace HireFlow.Application.Features.Auth.Commands.LoginCompany;

public class LoginCompanyValidator : AbstractValidator<LoginCompanyCommand>
{
    public LoginCompanyValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}