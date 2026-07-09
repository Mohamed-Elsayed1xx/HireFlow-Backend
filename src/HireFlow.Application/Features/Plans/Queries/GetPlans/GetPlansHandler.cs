using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace HireFlow.Application.Features.Plans.Queries.GetPlans;

public class GetPlansHandler : IRequestHandler<GetPlansQuery, Result<List<PlanDto>>>
{
    private readonly IPlanRepository _planRepository;

    public GetPlansHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }

    public async Task<Result<List<PlanDto>>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _planRepository.GetAllAsync();

        var result = plans
            .OrderBy(p => p.Price)
            .Select(p => p.ToDto())
            .ToList();

        return Result<List<PlanDto>>.Ok(result);
    }
}
