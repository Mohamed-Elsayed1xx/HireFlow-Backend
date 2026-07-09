using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repositories;

public interface IPasswordResetTokenRepository : IBaseRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task InvalidateAllForUserAsync(Guid userId);
}
