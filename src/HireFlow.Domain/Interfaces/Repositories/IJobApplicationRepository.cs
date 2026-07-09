using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repositories;

public interface IJobApplicationRepository : IBaseRepository<JobApplication>
{
    Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId);
    Task<IEnumerable<JobApplication>> GetByCandidateIdAsync(Guid candidateId);
    Task<IEnumerable<JobApplication>> GetByCompanyIdAsync(Guid companyId);
    Task<bool> ApplicationExistsAsync(Guid jobId, Guid candidateId);
    Task<bool> CandidateHasApplicationInCompanyAsync(Guid candidateId, Guid companyId);
    Task<JobApplication?> GetWithDetailsAsync(Guid id);
}