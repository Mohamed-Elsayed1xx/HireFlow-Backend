using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Commands.UpdateCandidateProfile;

public class UpdateCandidateProfileHandler : IRequestHandler<UpdateCandidateProfileCommand, Result<CandidateDto>>
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateCandidateProfileHandler(ICandidateRepository candidateRepository, ICurrentUserService currentUser)
    {
        _candidateRepository = candidateRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<CandidateDto>> Handle(
        UpdateCandidateProfileCommand request,
        CancellationToken cancellationToken)
    {
        var candidate = await _candidateRepository.GetWithProfileAsync(_currentUser.UserId);

        if (candidate is null)
            return Result<CandidateDto>.Fail("NOT_FOUND", "Profile not found.");

        candidate.FirstName = request.FirstName;
        candidate.LastName = request.LastName;
        candidate.Phone = request.Phone;
        candidate.HeadlineTitle = request.HeadlineTitle;
        candidate.Location = request.Location;
        candidate.LinkedInUrl = request.LinkedInUrl;
        candidate.PortfolioUrl = request.PortfolioUrl;
        candidate.UpdatedAt = DateTime.UtcNow;

        if (candidate.Profile is null)
        {
            var profile = new CandidateProfile
            {
                CandidateId = candidate.Id,
                Skills = request.Skills,
                Summary = request.Summary,
            };
            await _candidateRepository.AddProfileAsync(profile);
            candidate.Profile = profile;
        }
        else
        {
            candidate.Profile.Skills = request.Skills;
            candidate.Profile.Summary = request.Summary;
            candidate.Profile.UpdatedAt = DateTime.UtcNow;
        }

        await _candidateRepository.UpdateAsync(candidate);

        return Result<CandidateDto>.Ok(candidate.ToDto(), "Profile updated successfully.");
    }
}
