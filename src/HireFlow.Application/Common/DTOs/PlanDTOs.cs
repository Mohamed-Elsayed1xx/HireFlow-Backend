namespace HireFlow.Application.Common.DTOs;

public record PlanDto(
    Guid Id,
    string Name,
    decimal Price,
    int MaxJobs,
    int MaxUsers,
    int MaxCandidates,
    string? Features,
    bool IsActive,
    DateTime CreatedAt
);
