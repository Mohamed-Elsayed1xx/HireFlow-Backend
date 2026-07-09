using HireFlow.Application.Common.DTOs;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Common.Mappings;

public static class CandidateMappings
{
    public static CandidateDto ToDto(this Candidate candidate) => new(
        candidate.Id,
        candidate.FirstName,
        candidate.LastName,
        candidate.Email,
        candidate.Phone,
        candidate.HeadlineTitle,
        candidate.Location,
        candidate.LinkedInUrl,
        candidate.PortfolioUrl,
        candidate.Profile?.ToDto()
    );

    public static CandidateSummaryDto ToSummaryDto(this Candidate candidate) => new(
        candidate.Id,
        candidate.FirstName,
        candidate.LastName,
        candidate.Email,
        candidate.HeadlineTitle,
        candidate.Location
    );

    public static CandidateProfileDto ToDto(this CandidateProfile profile) => new(
        profile.Id,
        profile.CvUrl,
        profile.Skills,
        profile.Summary
    );
}
