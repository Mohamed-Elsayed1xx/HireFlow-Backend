using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Candidates.Queries.GetCandidateById;

public class GetCandidateByIdHandler : IRequestHandler<GetCandidateByIdQuery, Result<CandidateDto>>
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetCandidateByIdHandler(
        ICandidateRepository candidateRepository,
        IJobApplicationRepository applicationRepository,
        ICurrentUserService currentUser)
    {
        _candidateRepository = candidateRepository;
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<CandidateDto>> Handle(
        GetCandidateByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<CandidateDto>.Fail("NO_COMPANY", "User is not associated with a company.");

        var candidate = await _candidateRepository.GetWithProfileAsync(request.Id);

        if (candidate is null)
            return Result<CandidateDto>.Fail("NOT_FOUND", "Candidate not found.");

        var hasApplication = await _applicationRepository.CandidateHasApplicationInCompanyAsync(
            request.Id,
            _currentUser.CompanyId.Value);

        if (!hasApplication)
            return Result<CandidateDto>.Fail("FORBIDDEN", "Access denied.");

        return Result<CandidateDto>.Ok(candidate.ToDto());
    }
}