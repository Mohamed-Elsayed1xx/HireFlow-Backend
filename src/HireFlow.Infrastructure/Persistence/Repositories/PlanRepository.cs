using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HireFlow.Infrastructure.Persistence.Repositories;

public class PlanRepository : BaseRepository<Plan>, IPlanRepository
{
    public PlanRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Plan>> GetActiveAsync()
        => await _dbSet.Where(p => p.IsActive).ToListAsync();
}
