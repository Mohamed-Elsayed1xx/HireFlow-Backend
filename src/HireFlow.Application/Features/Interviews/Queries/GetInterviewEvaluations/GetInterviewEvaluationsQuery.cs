using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Queries.GetInterviewEvaluations;

public record GetInterviewEvaluationsQuery(Guid InterviewId) : IRequest<Result<List<EvaluationDto>>>;
