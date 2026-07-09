using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class Interview : BaseEntity
{
    public Guid ApplicationId { get; set; }
    public Guid ScheduledById { get; set; }
    public string Title { get; set; } = string.Empty;
    public InterviewType Type { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public string? Location { get; set; }
    public string? MeetingUrl { get; set; }
    public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    public string? Notes { get; set; }

    // Navigation
    public JobApplication Application { get; set; } = null!;
    public User ScheduledBy { get; set; } = null!;
    public ICollection<InterviewInterviewer> Interviewers { get; set; } = [];
    public ICollection<Evaluation> Evaluations { get; set; } = [];
}