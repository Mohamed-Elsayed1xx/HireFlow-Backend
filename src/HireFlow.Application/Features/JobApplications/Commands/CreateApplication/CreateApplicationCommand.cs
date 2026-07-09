using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Commands.CreateApplication;

public record CreateApplicationCommand(
    Guid JobId,
    string? CoverLetter,
    string? Source
) : IRequest<Result<JobApplicationDto>>;