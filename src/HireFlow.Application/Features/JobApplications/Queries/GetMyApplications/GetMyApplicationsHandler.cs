using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Queries.GetMyApplications;

public class GetMyApplicationsHandler : IRequestHandler<GetMyApplicationsQuery, Result<List<JobApplicationDto>>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetMyApplicationsHandler(IJobApplicationRepository applicationRepository, ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<JobApplicationDto>>> Handle(
        GetMyApplicationsQuery request,
        CancellationToken cancellationToken)
    {
        var applications = await _applicationRepository.GetByCandidateIdAsync(_currentUser.UserId);

        var result = applications
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => a.ToDto())
            .ToList();

        return Result<List<JobApplicationDto>>.Ok(result);
    }
}
