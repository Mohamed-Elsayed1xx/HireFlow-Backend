using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Commands.UpdateInterview;

public class UpdateInterviewHandler : IRequestHandler<UpdateInterviewCommand, Result<InterviewDto>>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateInterviewHandler(IInterviewRepository interviewRepository, ICurrentUserService currentUser)
    {
        _interviewRepository = interviewRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<InterviewDto>> Handle(UpdateInterviewCommand request, CancellationToken cancellationToken)
    {
        var interview = await _interviewRepository.GetWithDetailsAsync(request.Id);

        if (interview is null)
            return Result<InterviewDto>.Fail("NOT_FOUND", "Interview not found.");

        if (interview.Application.Job.CompanyId != _currentUser.CompanyId)
            return Result<InterviewDto>.Fail("FORBIDDEN", "Access denied.");

        interview.Title = request.Title;
        interview.Type = Enum.Parse<InterviewType>(request.Type);
        interview.ScheduledAt = request.ScheduledAt;
        interview.DurationMinutes = request.DurationMinutes;
        interview.Location = request.Location;
        interview.MeetingUrl = request.MeetingUrl;
        interview.Notes = request.Notes;
        interview.UpdatedAt = DateTime.UtcNow;

        await _interviewRepository.UpdateAsync(interview);

        return Result<InterviewDto>.Ok(interview.ToDto(), "Interview updated successfully.");
    }
}
