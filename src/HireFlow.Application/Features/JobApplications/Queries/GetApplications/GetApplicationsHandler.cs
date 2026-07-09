using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Queries.GetApplications;

public class GetApplicationsHandler : IRequestHandler<GetApplicationsQuery, Result<PagedResult<JobApplicationSummaryDto>>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetApplicationsHandler(
        IJobApplicationRepository applicationRepository,
        ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<JobApplicationSummaryDto>>> Handle(
        GetApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<PagedResult<JobApplicationSummaryDto>>.Fail("NO_COMPANY", "User is not associated with a company.");

        var applications = await _applicationRepository.GetByCompanyIdAsync(_currentUser.CompanyId.Value);

        if (_currentUser.Role == nameof(UserRole.HiringManager))
            applications = applications.Where(a => a.Job.Assignees.Any(asg => asg.UserId == _currentUser.UserId));

        if (request.JobId.HasValue)
            applications = applications.Where(a => a.JobId == request.JobId.Value);

        if (request.Stage is not null && Enum.TryParse<ApplicationStage>(request.Stage, out var stage))
            applications = applications.Where(a => a.Stage == stage);

        var totalCount = applications.Count();
        var items = applications
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => a.ToSummaryDto());

        return Result<PagedResult<JobApplicationSummaryDto>>.Ok(new PagedResult<JobApplicationSummaryDto>
        {
            Data = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}