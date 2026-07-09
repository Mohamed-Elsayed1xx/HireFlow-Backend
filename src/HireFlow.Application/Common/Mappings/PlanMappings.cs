using HireFlow.Application.Common.DTOs;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Common.Mappings;

public static class PlanMappings
{
    public static PlanDto ToDto(this Plan plan) => new(
        plan.Id,
        plan.Name,
        plan.Price,
        plan.MaxJobs,
        plan.MaxUsers,
        plan.MaxCandidates,
        plan.Features,
        plan.IsActive,
        plan.CreatedAt
    );
}
