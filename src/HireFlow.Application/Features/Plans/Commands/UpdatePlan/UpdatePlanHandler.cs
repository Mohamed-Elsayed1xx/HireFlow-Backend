using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Plans.Commands.UpdatePlan;

public class UpdatePlanHandler : IRequestHandler<UpdatePlanCommand, Result<PlanDto>>
{
    private readonly IPlanRepository _planRepository;
    private readonly IAuditService _auditService;

    public UpdatePlanHandler(IPlanRepository planRepository, IAuditService auditService)
    {
        _planRepository = planRepository;
        _auditService = auditService;
    }

    public async Task<Result<PlanDto>> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(request.Id);

        if (plan is null)
            return Result<PlanDto>.Fail("NOT_FOUND", "Plan not found.");

        plan.Name = request.Name;
        plan.Price = request.Price;
        plan.MaxJobs = request.MaxJobs;
        plan.MaxUsers = request.MaxUsers;
        plan.MaxCandidates = request.MaxCandidates;
        plan.Features = request.Features;
        plan.IsActive = request.IsActive;
        plan.UpdatedAt = DateTime.UtcNow;

        await _planRepository.UpdateAsync(plan);

        await _auditService.LogAsync("PlanUpdated", "Plan", plan.Id, null, plan.ToDto());

        return Result<PlanDto>.Ok(plan.ToDto(), "Plan updated successfully.");
    }
}
