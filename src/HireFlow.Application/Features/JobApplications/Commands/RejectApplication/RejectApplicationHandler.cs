using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Commands.RejectApplication;

public class RejectApplicationHandler : IRequestHandler<RejectApplicationCommand, Result>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public RejectApplicationHandler(
        IJobApplicationRepository applicationRepository,
        ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(RejectApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(request.ApplicationId);

        if (application is null)
            return Result.Fail("NOT_FOUND", "Application not found.");

        if (application.Job.CompanyId != _currentUser.CompanyId)
            return Result.Fail("FORBIDDEN", "Access denied.");

        if (application.Stage == ApplicationStage.Rejected)
            return Result.Fail("ALREADY_REJECTED", "Application is already rejected.");

        application.StageHistory.Add(new ApplicationStageHistory
        {
            ApplicationId = application.Id,
            FromStage = application.Stage,
            ToStage = ApplicationStage.Rejected,
            ChangedById = _currentUser.UserId,
            Note = request.Note,
            ChangedAt = DateTime.UtcNow
        });

        application.Stage = ApplicationStage.Rejected;
        application.RejectedAt = DateTime.UtcNow;
        application.RejectedById = _currentUser.UserId;
        application.UpdatedAt = DateTime.UtcNow;

        await _applicationRepository.UpdateAsync(application);

        return Result.Ok("Application rejected.");
    }
}