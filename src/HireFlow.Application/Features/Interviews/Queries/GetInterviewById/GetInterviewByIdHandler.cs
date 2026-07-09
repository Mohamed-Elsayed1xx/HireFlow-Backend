using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Queries.GetInterviewById;

public class GetInterviewByIdHandler : IRequestHandler<GetInterviewByIdQuery, Result<InterviewDto>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly ICurrentUserService _currentUser;

    public GetInterviewByIdHandler(
        IInterviewRepository interviewRepository,
        ICurrentUserService currentUser)
    {
        _interviewRepository = interviewRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<InterviewDto>> Handle(
        GetInterviewByIdQuery request,
        CancellationToken cancellationToken)
    {
        var interview = await _interviewRepository.GetWithDetailsAsync(request.Id);

        if (interview is null)
            return Result<InterviewDto>.Fail("NOT_FOUND", "Interview not found.");

        if (interview.Application.Job.CompanyId != _currentUser.CompanyId)
            return Result<InterviewDto>.Fail("FORBIDDEN", "Access denied.");

        return Result<InterviewDto>.Ok(interview.ToDto());
    }
}