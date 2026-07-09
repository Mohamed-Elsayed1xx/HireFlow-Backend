using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Plans.Commands.CreatePlan;

public class CreatePlanHandler : IRequestHandler<CreatePlanCommand, Result<PlanDto>>
{
    private readonly IPlanRepository _planRepository;
    private readonly IAuditService _auditService;

    public CreatePlanHandler(IPlanRepository planRepository, IAuditService auditService)
    {
        _planRepository = planRepository;
        _auditService = auditService;
    }

    public async Task<Result<PlanDto>> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = new Plan
        {
            Name = request.Name,
            Price = request.Price,
            MaxJobs = request.MaxJobs,
            MaxUsers = request.MaxUsers,
            MaxCandidates = request.MaxCandidates,
            Features = request.Features,
            IsActive = true
        };

        await _planRepository.AddAsync(plan);

        await _auditService.LogAsync("PlanCreated", "Plan", plan.Id, null, plan.ToDto());

        return Result<PlanDto>.Ok(plan.ToDto(), "Plan created successfully.");
    }
}
