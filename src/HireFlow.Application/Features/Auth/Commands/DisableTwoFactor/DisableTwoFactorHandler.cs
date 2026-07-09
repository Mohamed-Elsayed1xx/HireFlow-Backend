using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.DisableTwoFactor;

public class DisableTwoFactorHandler : IRequestHandler<DisableTwoFactorCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ITotpService _totpService;
    private readonly ICurrentUserService _currentUser;

    public DisableTwoFactorHandler(
        IUserRepository userRepository,
        ITotpService totpService,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _totpService = totpService;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId);

        if (user is null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
            return Result.Fail("NOT_ENABLED", "Two-factor authentication is not enabled.");

        if (!_totpService.ValidateCode(user.TwoFactorSecret, request.Code))
            return Result.Fail("INVALID_CODE", "Invalid authentication code.");

        user.TwoFactorEnabled = false;
        user.TwoFactorSecret = null;
        await _userRepository.UpdateAsync(user);

        return Result.Ok("Two-factor authentication disabled successfully.");
    }
}
