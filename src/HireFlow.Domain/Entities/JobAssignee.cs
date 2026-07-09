namespace HireFlow.Domain.Entities;

public class JobAssignee : BaseEntity
{
    public Guid JobId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Job Job { get; set; } = null!;
    public User User { get; set; } = null!;
}