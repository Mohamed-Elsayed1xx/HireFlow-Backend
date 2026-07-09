using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Queries.GetInterviews;

public class GetInterviewsHandler : IRequestHandler<GetInterviewsQuery, Result<PagedResult<InterviewDto>>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly ICurrentUserService _currentUser;

    public GetInterviewsHandler(
        IInterviewRepository interviewRepository,
        ICurrentUserService currentUser)
    {
        _interviewRepository = interviewRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<InterviewDto>>> Handle(
        GetInterviewsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<PagedResult<InterviewDto>>.Fail("NO_COMPANY", "User is not associated with a company.");

        var interviews = await _interviewRepository.GetByCompanyIdAsync(_currentUser.CompanyId.Value);

        if (_currentUser.Role == nameof(UserRole.HiringManager))
            interviews = interviews.Where(i => i.Application.Job.Assignees.Any(asg => asg.UserId == _currentUser.UserId));

        if (request.ApplicationId.HasValue)
            interviews = interviews.Where(i => i.ApplicationId == request.ApplicationId.Value);

        if (request.Status is not null && Enum.TryParse<InterviewStatus>(request.Status, out var status))
            interviews = interviews.Where(i => i.Status == status);

        var totalCount = interviews.Count();
        var items = interviews
            .OrderByDescending(i => i.ScheduledAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => i.ToDto());

        return Result<PagedResult<InterviewDto>>.Ok(new PagedResult<InterviewDto>
        {
            Data = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}