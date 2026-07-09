using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.UpdateJobStatus;

public record UpdateJobStatusCommand(Guid Id, string Status) : IRequest<Result<JobDto>>;