namespace HireFlow.Application.Common.DTOs;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    string Email,
    string Role
);

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    Guid? CompanyId,
    bool TwoFactorEnabled
);

/// <summary>
/// Returned by admin login. When the account has 2FA enabled, Tokens is null
/// and the client must call /api/auth/login/admin/verify-2fa with PendingToken
/// and the 6-digit code to receive real access/refresh tokens.
/// </summary>
public record AdminLoginResponse(
    bool RequiresTwoFactor,
    string? PendingToken,
    LoginResponse? Tokens
);

public record TwoFactorSetupResponse(
    string Secret,
    string AuthenticatorUri
);