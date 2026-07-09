using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Commands.BulkMoveApplications;

public class BulkMoveApplicationsHandler : IRequestHandler<BulkMoveApplicationsCommand, Result>
{
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly ICurrentUserService _currentUser;

    public BulkMoveApplicationsHandler(
        IJobApplicationRepository applicationRepository,
        ICurrentUserService currentUser)
    {
        _applicationRepository = applicationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(BulkMoveApplicationsCommand request, CancellationToken cancellationToken)
    {
        var newStage = Enum.Parse<ApplicationStage>(request.Stage);

        foreach (var applicationId in request.ApplicationIds)
        {
            var application = await _applicationRepository.GetWithDetailsAsync(applicationId);

            if (application is null || application.Job.CompanyId != _currentUser.CompanyId)
                continue;

            application.StageHistory.Add(new ApplicationStageHistory
            {
                ApplicationId = application.Id,
                FromStage = application.Stage,
                ToStage = newStage,
                ChangedById = _currentUser.UserId,
                ChangedAt = DateTime.UtcNow
            });

            application.Stage = newStage;
            application.UpdatedAt = DateTime.UtcNow;

            await _applicationRepository.UpdateAsync(application);
        }

        return Result.Ok("Applications moved successfully.");
    }
}