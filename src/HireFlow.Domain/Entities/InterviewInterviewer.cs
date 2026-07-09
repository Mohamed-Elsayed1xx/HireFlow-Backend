namespace HireFlow.Domain.Entities;

public class InterviewInterviewer : BaseEntity
{
    public Guid InterviewId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Interview Interview { get; set; } = null!;
    public User User { get; set; } = null!;
}