using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Team.Commands.UpdateMemberRole;

public record UpdateMemberRoleCommand(Guid MemberId, string Role) : IRequest<Result<TeamMemberDto>>;
