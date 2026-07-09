using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence.Repositories;

public class JobRepository : BaseRepository<Job>, IJobRepository
{
    public JobRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId)
        => await _dbSet
            .Include(j => j.Assignees)
            .Where(j => j.CompanyId == companyId)
            .ToListAsync();

    public async Task<Job?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(j => j.CreatedBy)
            .Include(j => j.Assignees).ThenInclude(a => a.User)
            .FirstOrDefaultAsync(j => j.Id == id);

    public async Task<IEnumerable<Job>> GetActivePublicAsync()
        => await _dbSet
            .Include(j => j.Company)
            .Where(j => j.Status == Domain.Enums.JobStatus.Active && j.Company.IsActive)
            .ToListAsync();

    public async Task<Job?> GetActivePublicByIdAsync(Guid id)
        => await _dbSet
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j =>
                j.Id == id &&
                j.Status == Domain.Enums.JobStatus.Active &&
                j.Company.IsActive);
}