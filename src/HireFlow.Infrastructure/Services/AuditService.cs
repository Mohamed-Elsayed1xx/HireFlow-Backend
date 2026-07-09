using System.Text.Json;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Services;

namespace HireFlow.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly ICurrentUserService _currentUser;
    private readonly Persistence.AppDbContext _context;

    public AuditService(ICurrentUserService currentUser, Persistence.AppDbContext context)
    {
        _currentUser = currentUser;
        _context = context;
    }

    public async Task LogAsync(string action, string entity, Guid entityId, object? oldValues = null, object? newValues = null)
    {
        var log = new AuditLog
        {
            UserId = _currentUser.UserId == Guid.Empty ? null : _currentUser.UserId,
            CompanyId = _currentUser.CompanyId,
            Action = action,
            Entity = entity,
            EntityId = entityId,
            OldValues = oldValues is not null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues is not null ? JsonSerializer.Serialize(newValues) : null
        };

        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}