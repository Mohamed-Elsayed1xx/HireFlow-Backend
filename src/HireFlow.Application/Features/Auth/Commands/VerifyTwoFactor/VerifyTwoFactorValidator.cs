using FluentValidation;

namespace HireFlow.Application.Features.Auth.Commands.VerifyTwoFactor;

public class VerifyTwoFactorValidator : AbstractValidator<VerifyTwoFactorCommand>
{
    public VerifyTwoFactorValidator()
    {
        RuleFor(x => x.PendingToken)
            .NotEmpty().WithMessage("Pending token is required.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Authentication code is required.")
            .Length(6).WithMessage("Authentication code must be 6 digits.");
    }
}
