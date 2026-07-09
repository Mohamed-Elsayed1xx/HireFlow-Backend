using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence.Repositories;

public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
{
    public CompanyRepository(AppDbContext context) : base(context) { }

    public async Task<Company?> GetBySlugAsync(string slug)
        => await _dbSet.FirstOrDefaultAsync(c => c.Slug == slug);

    public async Task<bool> SlugExistsAsync(string slug)
        => await _dbSet.AnyAsync(c => c.Slug == slug);

    public async Task<Company?> GetByIdWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(c => c.Plan)
            .Include(c => c.Users)
            .Include(c => c.Jobs)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Company>> GetAllWithDetailsAsync()
        => await _dbSet
            .Include(c => c.Plan)
            .Include(c => c.Users)
            .Include(c => c.Jobs)
            .ToListAsync();
}