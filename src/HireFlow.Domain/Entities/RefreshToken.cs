namespace HireFlow.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? CandidateId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;

    // Navigation
    public User? User { get; set; }
    public Candidate? Candidate { get; set; }
}