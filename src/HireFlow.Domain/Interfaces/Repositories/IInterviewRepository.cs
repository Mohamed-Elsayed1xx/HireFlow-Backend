using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repositories;

public interface IInterviewRepository : IBaseRepository<Interview>
{
    Task<IEnumerable<Interview>> GetByApplicationIdAsync(Guid applicationId);
    Task<IEnumerable<Interview>> GetByCompanyIdAsync(Guid companyId);
    Task<Interview?> GetWithDetailsAsync(Guid id);
}