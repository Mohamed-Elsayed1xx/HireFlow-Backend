using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Queries.GetCandidateById;

public record GetCandidateByIdQuery(Guid Id) : IRequest<Result<CandidateDto>>;