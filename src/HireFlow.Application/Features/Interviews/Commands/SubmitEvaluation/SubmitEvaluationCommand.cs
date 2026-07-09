using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Commands.SubmitEvaluation;

public record SubmitEvaluationCommand(
    Guid InterviewId,
    int Rating,
    int? TechnicalScore,
    int? CultureScore,
    int? CommunicationScore,
    string? Strengths,
    string? Weaknesses,
    string Recommendation,
    string? Notes
) : IRequest<Result<EvaluationDto>>;
