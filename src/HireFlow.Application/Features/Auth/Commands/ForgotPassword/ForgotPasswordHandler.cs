using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private const int TokenValidityHours = 1;
    private const string GenericSuccessMessage = "If an account exists with that email, a password reset link has been sent.";

    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;

    public ForgotPasswordHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository resetTokenRepository,
        ITokenService tokenService,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _resetTokenRepository = resetTokenRepository;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // Always return the same generic message, whether or not the account exists,
        // so this endpoint cannot be used to enumerate registered email addresses.
        if (user is null || !user.IsActive)
            return Result.Ok(GenericSuccessMessage);

        await _resetTokenRepository.InvalidateAllForUserAsync(user.Id);

        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddHours(TokenValidityHours)
        };

        await _resetTokenRepository.AddAsync(resetToken);

        await _emailService.SendAsync(
            user.Email,
            "Reset your HireFlow password",
            $"Hi {user.FirstName}, use this code to reset your password: {resetToken.Token}. This code expires in {TokenValidityHours} hour.");

        return Result.Ok(GenericSuccessMessage);
    }
}
