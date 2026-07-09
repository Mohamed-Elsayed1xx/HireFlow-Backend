using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Analytics.Queries.GetCandidatesOverTime;

public class GetCandidatesOverTimeHandler : IRequestHandler<GetCandidatesOverTimeQuery, Result<List<CandidatesOverTimeDto>>>
{
    private const int MonthsToInclude = 6;

    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetCandidatesOverTimeHandler(IJobApplicationRepository applicationRepository, ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<CandidatesOverTimeDto>>> Handle(
        GetCandidatesOverTimeQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<List<CandidatesOverTimeDto>>.Fail("NO_COMPANY", "User is not associated with a company.");

        var applications = await _applicationRepository.GetByCompanyIdAsync(_currentUser.CompanyId.Value);

        var rangeStart = DateTime.UtcNow.AddMonths(-(MonthsToInclude - 1));
        var monthBuckets = Enumerable.Range(0, MonthsToInclude)
            .Select(offset => rangeStart.AddMonths(offset))
            .Select(date => new DateTime(date.Year, date.Month, 1))
            .ToList();

        var grouped = applications
            .GroupBy(a => new DateTime(a.CreatedAt.Year, a.CreatedAt.Month, 1))
            .ToDictionary(g => g.Key, g => g.Select(a => a.CandidateId).Distinct().Count());

        var result = monthBuckets
            .Select(month => new CandidatesOverTimeDto(
                month.ToString("MMM yyyy"),
                grouped.GetValueOrDefault(month, 0)))
            .ToList();

        return Result<List<CandidatesOverTimeDto>>.Ok(result);
    }
}
