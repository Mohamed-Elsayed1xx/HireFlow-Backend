using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Queries.GetPublicJobs;

public class GetPublicJobsHandler : IRequestHandler<GetPublicJobsQuery, Result<PagedResult<PublicJobDto>>>
{
    private readonly IJobRepository _jobRepository;

    public GetPublicJobsHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<Result<PagedResult<PublicJobDto>>> Handle(
        GetPublicJobsQuery request,
        CancellationToken cancellationToken)
    {
        var jobs = await _jobRepository.GetActivePublicAsync();

        if (!string.IsNullOrWhiteSpace(request.Search))
            jobs = jobs.Where(j =>
                j.Title.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                j.Company.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.Location))
            jobs = jobs.Where(j =>
                j.Location != null && j.Location.Contains(request.Location, StringComparison.OrdinalIgnoreCase));

        var totalCount = jobs.Count();
        var items = jobs
            .OrderByDescending(j => j.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(j => j.ToPublicDto());

        return Result<PagedResult<PublicJobDto>>.Ok(new PagedResult<PublicJobDto>
        {
            Data = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}
