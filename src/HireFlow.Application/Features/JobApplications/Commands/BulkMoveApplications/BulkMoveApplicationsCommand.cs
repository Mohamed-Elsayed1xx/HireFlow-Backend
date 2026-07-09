using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Commands.BulkMoveApplications;

public record BulkMoveApplicationsCommand(
    List<Guid> ApplicationIds,
    string Stage
) : IRequest<Result>;
