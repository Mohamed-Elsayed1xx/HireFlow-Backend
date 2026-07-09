namespace HireFlow.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Industry { get; set; }
    public string? Size { get; set; }
    public Guid PlanId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Plan Plan { get; set; } = null!;
    public ICollection<User> Users { get; set; } = [];
    public ICollection<Job> Jobs { get; set; } = [];
    public ICollection<HiringStage> HiringStages { get; set; } = [];
}