namespace HireFlow.Domain.Interfaces.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entity, Guid entityId, object? oldValues = null, object? newValues = null);
}