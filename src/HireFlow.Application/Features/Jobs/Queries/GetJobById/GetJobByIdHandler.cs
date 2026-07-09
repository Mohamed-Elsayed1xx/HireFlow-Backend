using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Queries.GetJobById;

public class GetJobByIdHandler : IRequestHandler<GetJobByIdQuery, Result<JobDto>>
{
    private readonly IJobRepository _jobRepository;
    private readonly ICurrentUserService _currentUser;

    public GetJobByIdHandler(IJobRepository jobRepository, ICurrentUserService currentUser)
    {
        _jobRepository = jobRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<JobDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetWithDetailsAsync(request.Id);

        if (job is null)
            return Result<JobDto>.Fail("NOT_FOUND", "Job not found.");

        if (job.CompanyId != _currentUser.CompanyId)
            return Result<JobDto>.Fail("FORBIDDEN", "Access denied.");

        return Result<JobDto>.Ok(job.ToDto());
    }
}
