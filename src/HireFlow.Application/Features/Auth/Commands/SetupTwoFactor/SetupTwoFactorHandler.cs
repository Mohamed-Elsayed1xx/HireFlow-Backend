using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Auth.Commands.SetupTwoFactor;

public class SetupTwoFactorHandler : IRequestHandler<SetupTwoFactorCommand, Result<TwoFactorSetupResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITotpService _totpService;
    private readonly ICurrentUserService _currentUser;

    public SetupTwoFactorHandler(
        IUserRepository userRepository,
        ITotpService totpService,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _totpService = totpService;
        _currentUser = currentUser;
    }

    public async Task<Result<TwoFactorSetupResponse>> Handle(SetupTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId);

        if (user is null)
            return Result<TwoFactorSetupResponse>.Fail("NOT_FOUND", "User not found.");

        // Generating a new secret doesn't enable 2FA by itself — it only takes
        // effect once the user proves possession of it via /enable.
        var secret = _totpService.GenerateSecret();
        user.TwoFactorSecret = secret;
        await _userRepository.UpdateAsync(user);

        var uri = _totpService.BuildAuthenticatorUri(secret, user.Email);

        return Result<TwoFactorSetupResponse>.Ok(new TwoFactorSetupResponse(secret, uri));
    }
}
