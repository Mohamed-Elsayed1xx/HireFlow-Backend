using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Commands.UpdateApplicationStage;

public record UpdateApplicationStageCommand(
    Guid ApplicationId,
    string Stage,
    string? Note
) : IRequest<Result<JobApplicationDto>>;
