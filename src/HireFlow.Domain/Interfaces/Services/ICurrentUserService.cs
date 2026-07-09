namespace HireFlow.Domain.Interfaces.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid? CompanyId { get; }
    string Role { get; }
    string Email { get; }
}