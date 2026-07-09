using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Queries.GetCandidates;

public record GetCandidatesQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null
) : IRequest<Result<PagedResult<CandidateSummaryDto>>>;