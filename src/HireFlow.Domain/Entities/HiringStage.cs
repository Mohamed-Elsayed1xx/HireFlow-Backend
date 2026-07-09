namespace HireFlow.Domain.Entities;

public class HiringStage : BaseEntity
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? Color { get; set; }
    public bool IsDefault { get; set; } = false;

    // Navigation
    public Company Company { get; set; } = null!;
}