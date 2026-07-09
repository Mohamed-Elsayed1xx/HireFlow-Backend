using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IPasswordResetTokenRepository _resetTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly IAuditService _auditService;

    public ResetPasswordHandler(
        IPasswordResetTokenRepository resetTokenRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        IAuditService auditService)
    {
        _resetTokenRepository = resetTokenRepository;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetToken = await _resetTokenRepository.GetByTokenAsync(request.Token);

        if (resetToken is null || resetToken.IsUsed || resetToken.ExpiresAt < DateTime.UtcNow)
            return Result.Fail("INVALID_TOKEN", "This reset link is invalid or has expired.");

        var user = await _userRepository.GetByIdAsync(resetToken.UserId);

        if (user is null || !user.IsActive)
            return Result.Fail("INVALID_TOKEN", "This reset link is invalid or has expired.");

        user.PasswordHash = _passwordService.Hash(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        resetToken.IsUsed = true;
        await _resetTokenRepository.UpdateAsync(resetToken);

        await _auditService.LogAsync("PasswordReset", "User", user.Id);

        // Revoke all active sessions so the password change takes effect immediately everywhere.
        await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id);

        return Result.Ok("Password has been reset successfully.");
    }
}
