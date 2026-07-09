using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Analytics.Queries.GetCandidatesOverTime;

public record GetCandidatesOverTimeQuery : IRequest<Result<List<CandidatesOverTimeDto>>>;
