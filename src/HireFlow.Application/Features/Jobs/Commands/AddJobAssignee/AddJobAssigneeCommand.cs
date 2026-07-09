using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.AddJobAssignee;

public record AddJobAssigneeCommand(Guid JobId, Guid UserId, string Role) : IRequest<Result>;