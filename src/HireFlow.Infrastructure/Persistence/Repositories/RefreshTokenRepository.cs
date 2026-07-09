using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
        => await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await _dbSet
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllCandidateTokensAsync(Guid candidateId)
    {
        var tokens = await _dbSet
            .Where(rt => rt.CandidateId == candidateId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _context.SaveChangesAsync();
    }
}