using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.CreateJob;

public record CreateJobCommand(
    string Title,
    string? Department,
    string? Location,
    string Type,
    string? ExperienceLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string Description,
    string? Requirements,
    DateTime? ClosingDate
) : IRequest<Result<JobDto>>;