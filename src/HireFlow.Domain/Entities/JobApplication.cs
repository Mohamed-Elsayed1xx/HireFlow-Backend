using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class JobApplication : BaseEntity
{
    public Guid JobId { get; set; }
    public Guid CandidateId { get; set; }
    public string? CoverLetter { get; set; }
    public ApplicationStage Stage { get; set; } = ApplicationStage.Applied;
    public DateTime? RejectedAt { get; set; }
    public Guid? RejectedById { get; set; }
    public string? Source { get; set; }

    // Navigation
    public Job Job { get; set; } = null!;
    public Candidate Candidate { get; set; } = null!;
    public User? RejectedBy { get; set; }
    public ICollection<ApplicationStageHistory> StageHistory { get; set; } = [];
    public ICollection<Interview> Interviews { get; set; } = [];
}