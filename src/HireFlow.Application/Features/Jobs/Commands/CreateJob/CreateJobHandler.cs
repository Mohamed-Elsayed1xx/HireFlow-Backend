using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.CreateJob;

public class CreateJobHandler : IRequestHandler<CreateJobCommand, Result<JobDto>>
{
    private readonly IJobRepository _jobRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateJobHandler(IJobRepository jobRepository, ICurrentUserService currentUser)
    {
        _jobRepository = jobRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<JobDto>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            return Result<JobDto>.Fail("NO_COMPANY", "User is not associated with a company.");

        var job = new Job
        {
            CompanyId = _currentUser.CompanyId.Value,
            CreatedById = _currentUser.UserId,
            Title = request.Title,
            Department = request.Department,
            Location = request.Location,
            Type = Enum.Parse<JobType>(request.Type),
            ExperienceLevel = request.ExperienceLevel is not null
                ? Enum.Parse<ExperienceLevel>(request.ExperienceLevel)
                : null,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            Description = request.Description,
            Requirements = request.Requirements,
            Status = JobStatus.Draft,
            ClosingDate = request.ClosingDate
        };

        await _jobRepository.AddAsync(job);

        return Result<JobDto>.Ok(job.ToDto(), "Job created successfully.");
    }
}
