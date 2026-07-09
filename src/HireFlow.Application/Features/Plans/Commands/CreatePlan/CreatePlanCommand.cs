using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Plans.Commands.CreatePlan;

public record CreatePlanCommand(
    string Name,
    decimal Price,
    int MaxJobs,
    int MaxUsers,
    int MaxCandidates,
    string? Features
) : IRequest<Result<PlanDto>>;
