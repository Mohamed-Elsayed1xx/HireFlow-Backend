using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Commands.UpdateInterview;

public record UpdateInterviewCommand(
    Guid Id,
    string Title,
    string Type,
    DateTime ScheduledAt,
    int DurationMinutes,
    string? Location,
    string? MeetingUrl,
    string? Notes
) : IRequest<Result<InterviewDto>>;
