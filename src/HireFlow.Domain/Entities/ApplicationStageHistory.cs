using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class ApplicationStageHistory : BaseEntity
{
    public Guid ApplicationId { get; set; }
    public ApplicationStage? FromStage { get; set; }
    public ApplicationStage ToStage { get; set; }
    public Guid ChangedById { get; set; }
    public string? Note { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public JobApplication Application { get; set; } = null!;
    public User ChangedBy { get; set; } = null!;
}