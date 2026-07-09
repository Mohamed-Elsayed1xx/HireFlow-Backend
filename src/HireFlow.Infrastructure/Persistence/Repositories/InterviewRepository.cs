using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence.Repositories;

public class InterviewRepository : BaseRepository<Interview>, IInterviewRepository
{
    public InterviewRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Interview>> GetByApplicationIdAsync(Guid applicationId)
        => await _dbSet
            .Include(i => i.Interviewers).ThenInclude(ii => ii.User)
            .Include(i => i.Evaluations)
            .Where(i => i.ApplicationId == applicationId)
            .ToListAsync();

    public async Task<IEnumerable<Interview>> GetByCompanyIdAsync(Guid companyId)
        => await _dbSet
            .Include(i => i.Application).ThenInclude(a => a.Job).ThenInclude(j => j.Assignees)
            .Include(i => i.Interviewers).ThenInclude(ii => ii.User)
            .Include(i => i.Evaluations)
            .Where(i => i.Application.Job.CompanyId == companyId)
            .ToListAsync();

    public async Task<Interview?> GetWithDetailsAsync(Guid id)
        => await _dbSet
            .Include(i => i.Application).ThenInclude(a => a.Job)
            .Include(i => i.Application).ThenInclude(a => a.Candidate)
            .Include(i => i.ScheduledBy)
            .Include(i => i.Interviewers).ThenInclude(ii => ii.User)
            .Include(i => i.Evaluations).ThenInclude(e => e.Evaluator)
            .FirstOrDefaultAsync(i => i.Id == id);
}