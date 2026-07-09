using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.UpdateJob;

public class UpdateJobHandler : IRequestHandler<UpdateJobCommand, Result<JobDto>>
{
    private readonly IJobRepository _jobRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateJobHandler(IJobRepository jobRepository, ICurrentUserService currentUser)
    {
        _jobRepository = jobRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<JobDto>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.Id);

        if (job is null)
            return Result<JobDto>.Fail("NOT_FOUND", "Job not found.");

        if (job.CompanyId != _currentUser.CompanyId)
            return Result<JobDto>.Fail("FORBIDDEN", "Access denied.");

        job.Title = request.Title;
        job.Department = request.Department;
        job.Location = request.Location;
        job.Type = Enum.Parse<JobType>(request.Type);
        job.ExperienceLevel = request.ExperienceLevel is not null
            ? Enum.Parse<ExperienceLevel>(request.ExperienceLevel)
            : null;
        job.SalaryMin = request.SalaryMin;
        job.SalaryMax = request.SalaryMax;
        job.Description = request.Description;
        job.Requirements = request.Requirements;
        job.ClosingDate = request.ClosingDate;
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.UpdateAsync(job);

        return Result<JobDto>.Ok(job.ToDto());
    }
}
