using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;
using RefreshTokenEntity = HireFlow.Domain.Entities.RefreshToken;

namespace HireFlow.Application.Features.Auth.Commands.VerifyTwoFactor;

public class VerifyTwoFactorHandler : IRequestHandler<VerifyTwoFactorCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly ITotpService _totpService;

    public VerifyTwoFactorHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        ITotpService totpService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _totpService = totpService;
    }

    public async Task<Result<LoginResponse>> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var userId = _tokenService.ValidatePendingTwoFactorToken(request.PendingToken);

        if (userId is null)
            return Result<LoginResponse>.Fail("INVALID_TOKEN", "This login session has expired. Please log in again.");

        var user = await _userRepository.GetByIdAsync(userId.Value);

        if (user is null || !user.IsActive || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
            return Result<LoginResponse>.Fail("INVALID_TOKEN", "This login session has expired. Please log in again.");

        if (!_totpService.ValidateCode(user.TwoFactorSecret, request.Code))
            return Result<LoginResponse>.Fail("INVALID_CODE", "Invalid authentication code.");

        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.Role.ToString(), user.CompanyId);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        await _refreshTokenRepository.AddAsync(new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return Result<LoginResponse>.Ok(new LoginResponse(accessToken, refreshToken, user.Email, user.Role.ToString()));
    }
}
