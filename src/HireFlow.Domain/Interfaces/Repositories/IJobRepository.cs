using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repositories;

public interface IJobRepository : IBaseRepository<Job>
{
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId);
    Task<Job?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<Job>> GetActivePublicAsync();
    Task<Job?> GetActivePublicByIdAsync(Guid id);
}