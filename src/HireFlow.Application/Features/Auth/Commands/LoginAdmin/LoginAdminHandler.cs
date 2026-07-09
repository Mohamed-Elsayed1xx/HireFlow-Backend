using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;
using RefreshTokenEntity = HireFlow.Domain.Entities.RefreshToken;

namespace HireFlow.Application.Features.Auth.Commands.LoginAdmin;

public class LoginAdminHandler : IRequestHandler<LoginAdminCommand, Result<AdminLoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public LoginAdminHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<Result<AdminLoginResponse>> Handle(
        LoginAdminCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || user.Role != UserRole.SuperAdmin)
            return Result<AdminLoginResponse>.Fail("INVALID_CREDENTIALS", "Invalid email or password.");

        if (!user.IsActive)
            return Result<AdminLoginResponse>.Fail("ACCOUNT_DEACTIVATED", "Account is deactivated.");

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
            return Result<AdminLoginResponse>.Fail("INVALID_CREDENTIALS", "Invalid email or password.");

        if (user.TwoFactorEnabled)
        {
            var pendingToken = _tokenService.GeneratePendingTwoFactorToken(user.Id);
            return Result<AdminLoginResponse>.Ok(new AdminLoginResponse(true, pendingToken, null));
        }

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

        return Result<AdminLoginResponse>.Ok(new AdminLoginResponse(
            false,
            null,
            new LoginResponse(accessToken, refreshToken, user.Email, user.Role.ToString())
        ));
    }
}
