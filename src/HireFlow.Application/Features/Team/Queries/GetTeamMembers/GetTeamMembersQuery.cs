using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Team.Queries.GetTeamMembers;

public record GetTeamMembersQuery(
    int Page = 1,
    int PageSize = 20,
    string? Role = null,
    bool? IsActive = null
) : IRequest<Result<PagedResult<TeamMemberDto>>>;
