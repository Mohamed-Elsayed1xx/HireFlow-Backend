using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repositories;

public interface IPlanRepository : IBaseRepository<Plan>
{
    Task<IEnumerable<Plan>> GetActiveAsync();
}
