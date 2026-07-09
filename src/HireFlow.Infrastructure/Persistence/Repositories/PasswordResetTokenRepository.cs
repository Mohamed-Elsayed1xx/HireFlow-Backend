using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence.Repositories;

public class PasswordResetTokenRepository : BaseRepository<PasswordResetToken>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(AppDbContext context) : base(context) { }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
        => await _dbSet.FirstOrDefaultAsync(t => t.Token == token);

    public async Task InvalidateAllForUserAsync(Guid userId)
        => await _dbSet
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsUsed, true));
}
