using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Team.Commands.ActivateMember;

public record ActivateMemberCommand(Guid MemberId) : IRequest<Result>;
