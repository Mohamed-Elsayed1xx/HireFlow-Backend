namespace HireFlow.Domain.Interfaces.Services;

/// <summary>
/// Commits all pending changes tracked by the current database context.
/// Repositories stage changes (Add/Update/Remove) but never persist them
/// on their own — persistence happens exactly once per request, here.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
