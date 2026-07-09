using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.EnableTwoFactor;

public class EnableTwoFactorHandler : IRequestHandler<EnableTwoFactorCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ITotpService _totpService;
    private readonly ICurrentUserService _currentUser;

    public EnableTwoFactorHandler(
        IUserRepository userRepository,
        ITotpService totpService,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _totpService = totpService;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(EnableTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId);

        if (user is null || string.IsNullOrEmpty(user.TwoFactorSecret))
            return Result.Fail("NOT_FOUND", "Run /setup before enabling two-factor authentication.");

        if (!_totpService.ValidateCode(user.TwoFactorSecret, request.Code))
            return Result.Fail("INVALID_CODE", "Invalid authentication code.");

        user.TwoFactorEnabled = true;
        await _userRepository.UpdateAsync(user);

        return Result.Ok("Two-factor authentication enabled successfully.");
    }
}
