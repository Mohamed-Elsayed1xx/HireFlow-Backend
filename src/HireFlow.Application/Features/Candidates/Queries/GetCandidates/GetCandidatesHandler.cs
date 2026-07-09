using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Queries.GetCandidates;

public class GetCandidatesHandler : IRequestHandler<GetCandidatesQuery, Result<PagedResult<CandidateSummaryDto>>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ICurrentUserService _currentUser;

    public GetCandidatesHandler(
        IJobApplicationRepository applicationRepository,
        ICandidateRepository candidateRepository,
        ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _candidateRepository = candidateRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<CandidateSummaryDto>>> Handle(
        GetCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<PagedResult<CandidateSummaryDto>>.Fail("NO_COMPANY", "User is not associated with a company.");

        var applications = await _applicationRepository.GetByCompanyIdAsync(_currentUser.CompanyId.Value);

        if (_currentUser.Role == nameof(UserRole.HiringManager))
            applications = applications.Where(a => a.Job.Assignees.Any(asg => asg.UserId == _currentUser.UserId));

        var candidates = applications
            .Select(a => a.Candidate)
            .DistinctBy(c => c.Id);

        if (!string.IsNullOrWhiteSpace(request.Search))
            candidates = candidates.Where(c =>
                c.FirstName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                c.LastName.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

        var totalCount = candidates.Count();
        var items = candidates
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => c.ToSummaryDto());

        return Result<PagedResult<CandidateSummaryDto>>.Ok(new PagedResult<CandidateSummaryDto>
        {
            Data = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}