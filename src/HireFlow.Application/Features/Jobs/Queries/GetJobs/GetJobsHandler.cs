using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Queries.GetJobs;

public class GetJobsHandler : IRequestHandler<GetJobsQuery, Result<PagedResult<JobSummaryDto>>>
{
    private readonly IJobRepository _jobRepository;
    private readonly ICurrentUserService _currentUser;

    public GetJobsHandler(IJobRepository jobRepository, ICurrentUserService currentUser)
    {
        _jobRepository = jobRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<JobSummaryDto>>> Handle(GetJobsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<PagedResult<JobSummaryDto>>.Fail("NO_COMPANY", "User is not associated with a company.");

        var jobs = await _jobRepository.GetByCompanyIdAsync(_currentUser.CompanyId.Value);

        if (_currentUser.Role == nameof(UserRole.HiringManager))
            jobs = jobs.Where(j => j.Assignees.Any(a => a.UserId == _currentUser.UserId));

        if (request.Status is not null && Enum.TryParse<JobStatus>(request.Status, out var status))
            jobs = jobs.Where(j => j.Status == status);

        if (request.Type is not null && Enum.TryParse<JobType>(request.Type, out var type))
            jobs = jobs.Where(j => j.Type == type);

        if (!string.IsNullOrWhiteSpace(request.Search))
            jobs = jobs.Where(j =>
                j.Title.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                (j.Department != null && j.Department.Contains(request.Search, StringComparison.OrdinalIgnoreCase)));

        var totalCount = jobs.Count();
        var items = jobs
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(j => new JobSummaryDto(
                j.Id,
                j.Title,
                j.Department,
                j.Location,
                j.Type.ToString(),
                j.Status.ToString(),
                j.Applications.Count,
                j.CreatedAt
            ));

        return Result<PagedResult<JobSummaryDto>>.Ok(new PagedResult<JobSummaryDto>
        {
            Data = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}