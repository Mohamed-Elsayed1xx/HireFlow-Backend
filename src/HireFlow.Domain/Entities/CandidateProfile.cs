namespace HireFlow.Domain.Entities;

public class CandidateProfile : BaseEntity
{
    public Guid CandidateId { get; set; }
    public string? CvUrl { get; set; }
    public List<string> Skills { get; set; } = [];
    public string? Summary { get; set; }

    // Navigation
    public Candidate Candidate { get; set; } = null!;
}
