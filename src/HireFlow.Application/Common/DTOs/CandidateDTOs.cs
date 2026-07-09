namespace HireFlow.Application.Common.DTOs;

public record CandidateDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? HeadlineTitle,
    string? Location,
    string? LinkedInUrl,
    string? PortfolioUrl,
    CandidateProfileDto? Profile
);

public record CandidateProfileDto(
    Guid Id,
    string? CvUrl,
    List<string> Skills,
    string? Summary
);

public record CandidateSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? HeadlineTitle,
    string? Location
);
