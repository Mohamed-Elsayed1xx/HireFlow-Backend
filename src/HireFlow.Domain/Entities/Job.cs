using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class Job : BaseEntity
{
    public Guid CompanyId { get; set; }
    public Guid CreatedById { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Location { get; set; }
    public JobType Type { get; set; }
    public ExperienceLevel? ExperienceLevel { get; set; }
    public decimal? SalaryMin { get; set; }
    public decimal? SalaryMax { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Draft;
    public DateTime? ClosingDate { get; set; }

    // Navigation
    public Company Company { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public ICollection<JobAssignee> Assignees { get; set; } = [];
    public ICollection<JobApplication> Applications { get; set; } = [];
}