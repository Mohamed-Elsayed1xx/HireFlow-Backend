using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence.Repositories;

public class JobApplicationRepository : BaseRepository<JobApplication>, IJobApplicationRepository
{
    public JobApplicationRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId)
        => await _dbSet.Where(a => a.JobId == jobId).Include(a => a.Candidate).ToListAsync();

    public async Task<IEnumerable<JobApplication>> GetByCandidateIdAsync(Guid candidateId)
        => await _dbSet.Where(a => a.CandidateId == candidateId).Include(a => a.Job).ToListAsync();

    public async Task<IEnumerable<JobApplication>> GetByCompanyIdAsync(Guid companyId)
        => await _dbSet
            .Include(a => a.Candidate)
            .Include(a => a.Job).ThenInclude(j => j.Assignees)
            .Where(a => a.Job.CompanyId == companyId)
            .ToListAsync();

    public async Task<bool> ApplicationExistsAsync(Guid jobId, Guid candidateId)
        => await _dbSet.AnyAsync(a => a.JobId == jobId && a.CandidateId == candidateId);

    public async Task<bool> CandidateHasApplicationInCompanyAsync(Guid candidateId, Guid companyId)
        => await _dbSet.AnyAsync(a => a.CandidateId == candidateId && a.Job.CompanyId == companyId);

    public async Task<JobApplication?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(a => a.Job)
            .Include(a => a.Candidate)
            .Include(a => a.StageHistory).ThenInclude(h => h.ChangedBy)
            .Include(a => a.Interviews)
            .FirstOrDefaultAsync(a => a.Id == id);
}
