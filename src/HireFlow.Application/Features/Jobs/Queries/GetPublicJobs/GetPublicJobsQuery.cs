using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Queries.GetPublicJobs;

public record GetPublicJobsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? Location = null
) : IRequest<Result<PagedResult<PublicJobDto>>>;
