using HireFlow.Application.Common.DTOs;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Common.Mappings;

public static class TeamMappings
{
    public static TeamMemberDto ToDto(this User user) => new(
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.Role.ToString(),
        user.IsActive,
        user.LastLoginAt,
        user.CreatedAt
    );
}
