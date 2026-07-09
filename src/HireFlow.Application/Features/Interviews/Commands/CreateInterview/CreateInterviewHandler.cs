using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Commands.CreateInterview;

public class CreateInterviewHandler : IRequestHandler<CreateInterviewCommand, Result<InterviewDto>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly IJobApplicationRepository _applicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateInterviewHandler(
        IInterviewRepository interviewRepository,
        IJobApplicationRepository applicationRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUser)
    {
        _interviewRepository = interviewRepository;
        _applicationRepository = applicationRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<InterviewDto>> Handle(CreateInterviewCommand request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetWithDetailsAsync(request.ApplicationId);

        if (application is null)
            return Result<InterviewDto>.Fail("NOT_FOUND", "Job application not found.");

        if (application.Job.CompanyId != _currentUser.CompanyId)
            return Result<InterviewDto>.Fail("FORBIDDEN", "Access denied.");

        var interviewers = new List<InterviewInterviewer>();
        foreach (var userId in request.InterviewerIds)
        {
            var interviewer = await _userRepository.GetByIdAsync(userId);
            if (interviewer is not null)
                interviewers.Add(new InterviewInterviewer { UserId = userId, User = interviewer });
        }

        var interview = new Interview
        {
            ApplicationId = request.ApplicationId,
            ScheduledById = _currentUser.UserId,
            Title = request.Title,
            Type = Enum.Parse<InterviewType>(request.Type),
            ScheduledAt = request.ScheduledAt,
            DurationMinutes = request.DurationMinutes,
            Location = request.Location,
            MeetingUrl = request.MeetingUrl,
            Status = InterviewStatus.Scheduled,
            Notes = request.Notes,
            Interviewers = interviewers
        };

        await _interviewRepository.AddAsync(interview);

        // SaveChanges hasn't run yet (it runs after the handler returns, via
        // UnitOfWorkBehavior), so re-querying the DB here for the just-added
        // row would find nothing. Map directly from what's already in memory.
        return Result<InterviewDto>.Ok(interview.ToDto(), "Interview scheduled successfully.");
    }
}
