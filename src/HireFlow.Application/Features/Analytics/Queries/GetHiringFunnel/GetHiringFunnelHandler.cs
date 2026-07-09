using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Analytics.Queries.GetHiringFunnel;

public class GetHiringFunnelHandler : IRequestHandler<GetHiringFunnelQuery, Result<List<HiringFunnelDto>>>
{
    private static readonly ApplicationStage[] FunnelOrder =
    {
        ApplicationStage.Applied,
        ApplicationStage.Screening,
        ApplicationStage.Interview,
        ApplicationStage.Assessment,
        ApplicationStage.Offer,
        ApplicationStage.Hired
    };

    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetHiringFunnelHandler(IJobApplicationRepository applicationRepository, ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<HiringFunnelDto>>> Handle(
        GetHiringFunnelQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<List<HiringFunnelDto>>.Fail("NO_COMPANY", "User is not associated with a company.");

        var applications = (await _applicationRepository.GetByCompanyIdAsync(_currentUser.CompanyId.Value)).ToList();
        var totalCount = applications.Count;

        var funnel = FunnelOrder.Select(stage =>
        {
            var count = applications.Count(a => a.Stage == stage);
            var percentage = totalCount > 0 ? Math.Round(count * 100.0 / totalCount, 1) : 0;

            return new HiringFunnelDto(stage.ToString(), count, percentage);
        }).ToList();

        return Result<List<HiringFunnelDto>>.Ok(funnel);
    }
}
