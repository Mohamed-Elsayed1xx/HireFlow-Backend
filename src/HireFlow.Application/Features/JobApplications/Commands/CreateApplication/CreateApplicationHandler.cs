using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Commands.CreateApplication;

public class CreateApplicationHandler : IRequestHandler<CreateApplicationCommand, Result<JobApplicationDto>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateApplicationHandler(
        IJobApplicationRepository applicationRepository,
        IJobRepository jobRepository,
        ICandidateRepository candidateRepository,
        ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
        _candidateRepository = candidateRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<JobApplicationDto>> Handle(
        CreateApplicationCommand request,
        CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.JobId);

        if (job is null || job.Status != JobStatus.Active)
            return Result<JobApplicationDto>.Fail("NOT_FOUND", "Job not found or not active.");

        var alreadyApplied = await _applicationRepository.ApplicationExistsAsync(
            request.JobId,
            _currentUser.UserId);

        if (alreadyApplied)
            return Result<JobApplicationDto>.Fail("ALREADY_APPLIED", "You have already applied for this job.");

        var candidate = await _candidateRepository.GetByIdAsync(_currentUser.UserId);

        var application = new JobApplication
        {
            JobId = request.JobId,
            CandidateId = _currentUser.UserId,
            CoverLetter = request.CoverLetter,
            Stage = ApplicationStage.Applied,
            Source = request.Source
        };

        await _applicationRepository.AddAsync(application);

        // SaveChanges hasn't run yet at this point (it runs after the handler
        // returns, via UnitOfWorkBehavior), so re-querying the DB here for
        // the just-added row would find nothing. Attach what we already have
        // in memory instead of re-fetching.
        application.Job = job;
        application.Candidate = candidate;

        return Result<JobApplicationDto>.Ok(application.ToDto(), "Application submitted successfully.");
    }
}