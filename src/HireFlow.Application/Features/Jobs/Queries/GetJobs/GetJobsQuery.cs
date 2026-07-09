using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Queries.GetJobs;

public record GetJobsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Status = null,
    string? Type = null,
    string? Search = null
) : IRequest<Result<PagedResult<JobSummaryDto>>>;