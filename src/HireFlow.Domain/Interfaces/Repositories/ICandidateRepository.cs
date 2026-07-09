using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repositories;

public interface ICandidateRepository : IBaseRepository<Candidate>
{
    Task<Candidate?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<Candidate?> GetWithProfileAsync(Guid id);
    Task AddProfileAsync(CandidateProfile profile);
}