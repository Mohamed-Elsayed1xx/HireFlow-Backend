using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Plans.Commands.UpdatePlan;

public record UpdatePlanCommand(
    Guid Id,
    string Name,
    decimal Price,
    int MaxJobs,
    int MaxUsers,
    int MaxCandidates,
    string? Features,
    bool IsActive
) : IRequest<Result<PlanDto>>;
