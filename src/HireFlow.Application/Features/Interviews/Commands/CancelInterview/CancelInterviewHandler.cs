using HireFlow.Application.Common.Models;
using HireFlow.Domain.Enums;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Commands.CancelInterview;

public class CancelInterviewHandler : IRequestHandler<CancelInterviewCommand, Result>
{
    private readonly IInterviewRepository _interviewRepository;
    private readonly ICurrentUserService _currentUser;

    public CancelInterviewHandler(
        IInterviewRepository interviewRepository,
        ICurrentUserService currentUser)
    {
        _interviewRepository = interviewRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CancelInterviewCommand request, CancellationToken cancellationToken)
    {
        var interview = await _interviewRepository.GetWithDetailsAsync(request.Id);

        if (interview is null)
            return Result.Fail("NOT_FOUND", "Interview not found.");

        if (interview.Application.Job.CompanyId != _currentUser.CompanyId)
            return Result.Fail("FORBIDDEN", "Access denied.");

        if (interview.Status == InterviewStatus.Cancelled)
            return Result.Fail("ALREADY_CANCELLED", "Interview is already cancelled.");

        interview.Status = InterviewStatus.Cancelled;
        interview.UpdatedAt = DateTime.UtcNow;

        await _interviewRepository.UpdateAsync(interview);

        return Result.Ok("Interview cancelled successfully.");
    }
}