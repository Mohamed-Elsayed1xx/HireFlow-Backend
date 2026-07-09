using FluentValidation;

namespace HireFlow.Application.Features.Auth.Commands.LoginCandidate;

public class LoginCandidateValidator : AbstractValidator<LoginCandidateCommand>
{
    public LoginCandidateValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}