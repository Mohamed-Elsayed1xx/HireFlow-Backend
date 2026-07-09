using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Commands.SubmitEvaluation;

public class SubmitEvaluationHandler : IRequestHandler<SubmitEvaluationCommand, Result<EvaluationDto>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly ICurrentUserService _currentUser;

    public SubmitEvaluationHandler(IInterviewRepository interviewRepository, ICurrentUserService currentUser)
    {
        _interviewRepository = interviewRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<EvaluationDto>> Handle(SubmitEvaluationCommand request, CancellationToken cancellationToken)
    {
        var interview = await _interviewRepository.GetWithDetailsAsync(request.InterviewId);

        if (interview is null)
            return Result<EvaluationDto>.Fail("NOT_FOUND", "Interview not found.");

        if (interview.Application.Job.CompanyId != _currentUser.CompanyId)
            return Result<EvaluationDto>.Fail("FORBIDDEN", "Access denied.");

        if (interview.Evaluations.Any(e => e.EvaluatorId == _currentUser.UserId))
            return Result<EvaluationDto>.Fail("ALREADY_EVALUATED", "You have already submitted an evaluation for this interview.");

        var evaluation = new Evaluation
        {
            InterviewId = request.InterviewId,
            EvaluatorId = _currentUser.UserId,
            Rating = request.Rating,
            TechnicalScore = request.TechnicalScore,
            CultureScore = request.CultureScore,
            CommunicationScore = request.CommunicationScore,
            Strengths = request.Strengths,
            Weaknesses = request.Weaknesses,
            Recommendation = Enum.Parse<EvaluationRecommendation>(request.Recommendation),
            Notes = request.Notes
        };

        interview.Evaluations.Add(evaluation);
        await _interviewRepository.UpdateAsync(interview);

        var savedInterview = await _interviewRepository.GetWithDetailsAsync(request.InterviewId);
        var savedEvaluation = savedInterview!.Evaluations.First(e => e.Id == evaluation.Id);

        return Result<EvaluationDto>.Ok(savedEvaluation.ToDto(), "Evaluation submitted successfully.");
    }
}
