using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Commands.CreateInterview;

public record CreateInterviewCommand(
    Guid ApplicationId,
    string Title,
    string Type,
    DateTime ScheduledAt,
    int DurationMinutes,
    string? Location,
    string? MeetingUrl,
    string? Notes,
    List<Guid> InterviewerIds
) : IRequest<Result<InterviewDto>>;
