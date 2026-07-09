using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Queries.GetApplications;

public record GetApplicationsQuery(
    Guid? JobId = null,
    string? Stage = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<JobApplicationSummaryDto>>>;
