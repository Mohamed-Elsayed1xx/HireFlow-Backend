using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Queries.GetInterviewEvaluations;

public class GetInterviewEvaluationsHandler : IRequestHandler<GetInterviewEvaluationsQuery, Result<List<EvaluationDto>>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly ICurrentUserService _currentUser;

    public GetInterviewEvaluationsHandler(IInterviewRepository interviewRepository, ICurrentUserService currentUser)
    {
        _interviewRepository = interviewRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<EvaluationDto>>> Handle(
        GetInterviewEvaluationsQuery request,
        CancellationToken cancellationToken)
    {
        var interview = await _interviewRepository.GetWithDetailsAsync(request.InterviewId);

        if (interview is null)
            return Result<List<EvaluationDto>>.Fail("NOT_FOUND", "Interview not found.");

        if (interview.Application.Job.CompanyId != _currentUser.CompanyId)
            return Result<List<EvaluationDto>>.Fail("FORBIDDEN", "Access denied.");

        var evaluations = interview.Evaluations.Select(e => e.ToDto()).ToList();

        return Result<List<EvaluationDto>>.Ok(evaluations);
    }
}
