using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Commands.UpdateCandidateProfile;

public record UpdateCandidateProfileCommand(
    string FirstName,
    string LastName,
    string? Phone,
    string? HeadlineTitle,
    string? Location,
    string? LinkedInUrl,
    string? PortfolioUrl,
    List<string> Skills,
    string? Summary
) : IRequest<Result<CandidateDto>>;
