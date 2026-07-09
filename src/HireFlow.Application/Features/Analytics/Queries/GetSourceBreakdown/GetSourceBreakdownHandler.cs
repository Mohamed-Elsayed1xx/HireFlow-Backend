using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Analytics.Queries.GetSourceBreakdown;

public class GetSourceBreakdownHandler : IRequestHandler<GetSourceBreakdownQuery, Result<List<SourceBreakdownDto>>>
{
    private const string UnknownSourceLabel = "Direct";

    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetSourceBreakdownHandler(IJobApplicationRepository applicationRepository, ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<SourceBreakdownDto>>> Handle(
        GetSourceBreakdownQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<List<SourceBreakdownDto>>.Fail("NO_COMPANY", "User is not associated with a company.");

        var applications = (await _applicationRepository.GetByCompanyIdAsync(_currentUser.CompanyId.Value)).ToList();
        var totalCount = applications.Count;

        var breakdown = applications
            .GroupBy(a => string.IsNullOrWhiteSpace(a.Source) ? UnknownSourceLabel : a.Source!)
            .Select(g => new SourceBreakdownDto(
                g.Key,
                g.Count(),
                totalCount > 0 ? Math.Round(g.Count() * 100.0 / totalCount, 1) : 0))
            .OrderByDescending(s => s.Count)
            .ToList();

        return Result<List<SourceBreakdownDto>>.Ok(breakdown);
    }
}
