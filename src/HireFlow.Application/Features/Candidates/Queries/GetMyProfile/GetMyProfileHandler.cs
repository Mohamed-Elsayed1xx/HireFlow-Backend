using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Queries.GetMyProfile;

public class GetMyProfileHandler : IRequestHandler<GetMyProfileQuery, Result<CandidateDto>>
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly ICurrentUserService _currentUser;

    public GetMyProfileHandler(ICandidateRepository candidateRepository, ICurrentUserService currentUser)
    {
        _candidateRepository = candidateRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<CandidateDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetWithProfileAsync(_currentUser.UserId);

        if (candidate is null)
            return Result<CandidateDto>.Fail("NOT_FOUND", "Profile not found.");

        return Result<CandidateDto>.Ok(candidate.ToDto());
    }
}
