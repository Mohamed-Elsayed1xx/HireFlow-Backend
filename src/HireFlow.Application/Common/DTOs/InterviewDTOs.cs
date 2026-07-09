namespace HireFlow.Application.Common.DTOs;

public record InterviewDto(
    Guid Id,
    Guid ApplicationId,
    string Title,
    string Type,
    DateTime ScheduledAt,
    int DurationMinutes,
    string? Location,
    string? MeetingUrl,
    string Status,
    string? Notes,
    List<InterviewerDto> Interviewers,
    DateTime CreatedAt
);

public record InterviewerDto(
    Guid UserId,
    string FirstName,
    string LastName
);

public record EvaluationDto(
    Guid Id,
    Guid InterviewId,
    Guid EvaluatorId,
    string EvaluatorName,
    int Rating,
    int? TechnicalScore,
    int? CultureScore,
    int? CommunicationScore,
    string? Strengths,
    string? Weaknesses,
    string Recommendation,
    string? Notes,
    DateTime CreatedAt
);