using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Team.Commands.DeactivateMember;

public record DeactivateMemberCommand(Guid MemberId) : IRequest<Result>;
