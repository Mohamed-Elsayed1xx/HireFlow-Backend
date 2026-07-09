using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.UpdateJobStatus;

public class UpdateJobStatusHandler : IRequestHandler<UpdateJobStatusCommand, Result<JobDto>>
{
    private readonly IJobRepository _jobRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateJobStatusHandler(IJobRepository jobRepository, ICurrentUserService currentUser)
    {
        _jobRepository = jobRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<JobDto>> Handle(UpdateJobStatusCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.Id);

        if (job is null)
            return Result<JobDto>.Fail("NOT_FOUND", "Job not found.");

        if (job.CompanyId != _currentUser.CompanyId)
            return Result<JobDto>.Fail("FORBIDDEN", "Access denied.");

        job.Status = Enum.Parse<JobStatus>(request.Status);
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.UpdateAsync(job);

        return Result<JobDto>.Ok(job.ToDto());
    }
}
