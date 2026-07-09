using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Team.Commands.InviteTeamMember;

public record InviteTeamMemberCommand(
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string Password
) : IRequest<Result<TeamMemberDto>>;
