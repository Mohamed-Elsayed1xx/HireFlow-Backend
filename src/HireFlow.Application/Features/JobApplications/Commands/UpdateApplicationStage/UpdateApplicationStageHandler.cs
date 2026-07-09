using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Commands.UpdateApplicationStage;

public class UpdateApplicationStageHandler : IRequestHandler<UpdateApplicationStageCommand, Result<JobApplicationDto>>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateApplicationStageHandler(
        IJobApplicationRepository applicationRepository,
        ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<JobApplicationDto>> Handle(
        UpdateApplicationStageCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(request.ApplicationId);

        if (application is null)
            return Result<JobApplicationDto>.Fail("NOT_FOUND", "Application not found.");

        if (application.Job.CompanyId != _currentUser.CompanyId)
            return Result<JobApplicationDto>.Fail("FORBIDDEN", "Access denied.");

        if (!Enum.TryParse<ApplicationStage>(request.Stage, ignoreCase: true, out var newStage))
            return Result<JobApplicationDto>.Fail("INVALID_STAGE", $"'{request.Stage}' is not a valid application stage.");

        application.StageHistory.Add(new ApplicationStageHistory
        {
            ApplicationId = application.Id,
            FromStage = application.Stage,
            ToStage = newStage,
            ChangedById = _currentUser.UserId,
            Note = request.Note,
            ChangedAt = DateTime.UtcNow
        });

        application.Stage = newStage;
        application.UpdatedAt = DateTime.UtcNow;

        await _applicationRepository.UpdateAsync(application);

        return Result<JobApplicationDto>.Ok(application.ToDto());
    }
}