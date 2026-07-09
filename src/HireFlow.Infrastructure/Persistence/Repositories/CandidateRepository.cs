using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence.Repositories;

public class CandidateRepository : BaseRepository<Candidate>, ICandidateRepository
{
    public CandidateRepository(AppDbContext context) : base(context) { }

    public async Task<Candidate?> GetByEmailAsync(string email)
        => await _dbSet.FirstOrDefaultAsync(c => c.Email == email);

    public async Task<bool> EmailExistsAsync(string email)
        => await _dbSet.AnyAsync(c => c.Email == email);

    public async Task<Candidate?> GetWithProfileAsync(Guid id)
        => await _dbSet
            .Include(c => c.Profile)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task AddProfileAsync(CandidateProfile profile)
        => await _context.Set<CandidateProfile>().AddAsync(profile);
}