using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<User>> GetByCompanyIdAsync(Guid companyId);
}