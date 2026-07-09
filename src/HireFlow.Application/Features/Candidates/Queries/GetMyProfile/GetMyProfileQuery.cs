using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Queries.GetMyProfile;

public record GetMyProfileQuery : IRequest<Result<CandidateDto>>;
