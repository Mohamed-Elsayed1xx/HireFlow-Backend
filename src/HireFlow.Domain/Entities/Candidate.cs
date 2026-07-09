namespace HireFlow.Domain.Entities;

public class Candidate : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? HeadlineTitle { get; set; }
    public string? Location { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? PortfolioUrl { get; set; }

    // Navigation
    public CandidateProfile? Profile { get; set; }
    public ICollection<JobApplication> Applications { get; set; } = [];
}