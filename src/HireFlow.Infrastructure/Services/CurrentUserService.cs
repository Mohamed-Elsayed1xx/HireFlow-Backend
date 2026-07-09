using System.Security.Claims;
using HireFlow.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace HireFlow.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext!.User;

    public Guid UserId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            return string.IsNullOrEmpty(value) ? Guid.Empty : Guid.Parse(value);
        }
    }

    public Guid? CompanyId
    {
        get
        {
            var value = User.FindFirstValue("companyId");
            return string.IsNullOrEmpty(value) ? null : Guid.Parse(value);
        }
    }

    public string Role
        => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public string Email
        => User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
}