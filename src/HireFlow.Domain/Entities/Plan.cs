namespace HireFlow.Domain.Entities;

public class Plan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxJobs { get; set; }
    public int MaxUsers { get; set; }
    public int MaxCandidates { get; set; }
    public string? Features { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Company> Companies { get; set; } = [];
}