using HireFlow.Domain.Entities;

namespace HireFlow.Domain.Interfaces.Repositories;

public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company?> GetBySlugAsync(string slug);
    Task<bool> SlugExistsAsync(string slug);
    Task<Company?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Company>> GetAllWithDetailsAsync();
}