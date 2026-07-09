using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Queries.GetInterviews;

public record GetInterviewsQuery(
    Guid? ApplicationId = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PagedResult<InterviewDto>>>;
