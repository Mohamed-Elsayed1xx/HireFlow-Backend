using HireFlow.Domain.Enums;

namespace HireFlow.Domain.Entities;

public class Evaluation : BaseEntity
{
    public Guid InterviewId { get; set; }
    public Guid EvaluatorId { get; set; }
    public int Rating { get; set; }
    public int? TechnicalScore { get; set; }
    public int? CultureScore { get; set; }
    public int? CommunicationScore { get; set; }
    public string? Strengths { get; set; }
    public string? Weaknesses { get; set; }
    public EvaluationRecommendation Recommendation { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Interview Interview { get; set; } = null!;
    public User Evaluator { get; set; } = null!;
}