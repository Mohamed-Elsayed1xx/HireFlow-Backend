using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email, string role, Guid? companyId = null);
    string GenerateRefreshToken();
    Guid? GetUserIdFromToken(string token);
    string GeneratePendingTwoFactorToken(Guid userId);
    Guid? ValidatePendingTwoFactorToken(string token);
}