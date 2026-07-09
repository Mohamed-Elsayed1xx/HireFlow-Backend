using HireFlow.Application.Common.DTOs;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Common.Mappings;

public static class InterviewMappings
{
    public static InterviewDto ToDto(this Interview interview) => new(
        interview.Id,
        interview.ApplicationId,
        interview.Title,
        interview.Type.ToString(),
        interview.ScheduledAt,
        interview.DurationMinutes,
        interview.Location,
        interview.MeetingUrl,
        interview.Status.ToString(),
        interview.Notes,
        interview.Interviewers.Select(ii => new InterviewerDto(
            ii.UserId,
            ii.User.FirstName,
            ii.User.LastName
        )).ToList(),
        interview.CreatedAt
    );

    public static EvaluationDto ToDto(this Evaluation evaluation) => new(
        evaluation.Id,
        evaluation.InterviewId,
        evaluation.EvaluatorId,
        $"{evaluation.Evaluator.FirstName} {evaluation.Evaluator.LastName}",
        evaluation.Rating,
        evaluation.TechnicalScore,
        evaluation.CultureScore,
        evaluation.CommunicationScore,
        evaluation.Strengths,
        evaluation.Weaknesses,
        evaluation.Recommendation.ToString(),
        evaluation.Notes,
        evaluation.CreatedAt
    );
}