using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Queries.GetApplicationById;

public class GetApplicationByIdHandler : IRequestHandler<GetApplicationByIdQuery, Result<JobApplicationDto>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetApplicationByIdHandler(IJobApplicationRepository applicationRepository, ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<JobApplicationDto>> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(request.Id);

        if (application is null)
            return Result<JobApplicationDto>.Fail("NOT_FOUND", "Job application not found.");

        if (application.Job.CompanyId != _currentUser.CompanyId)
            return Result<JobApplicationDto>.Fail("FORBIDDEN", "Access denied.");

        return Result<JobApplicationDto>.Ok(application.ToDto());
    }
}
