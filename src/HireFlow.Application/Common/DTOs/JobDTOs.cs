namespace HireFlow.Application.Common.DTOs;

public record JobDto(
    Guid Id,
    Guid CompanyId,
    Guid CreatedById,
    string Title,
    string? Department,
    string? Location,
    string Type,
    string? ExperienceLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string Description,
    string? Requirements,
    string Status,
    DateTime? ClosingDate,
    DateTime CreatedAt
);

public record JobSummaryDto(
    Guid Id,
    string Title,
    string? Department,
    string? Location,
    string Type,
    string Status,
    int ApplicationCount,
    DateTime CreatedAt
);

public record JobAssigneeDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Role
);
public record PublicJobDto(
    Guid Id,
    string CompanyName,
    string Title,
    string? Department,
    string? Location,
    string Type,
    string? ExperienceLevel,
    decimal? SalaryMin,
    decimal? SalaryMax,
    string Description,
    string? Requirements,
    DateTime? ClosingDate,
    DateTime CreatedAt
);
