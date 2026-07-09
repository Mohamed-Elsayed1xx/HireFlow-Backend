namespace HireFlow.Application.Common.DTOs;

public record CompanyDto(
    Guid Id,
    string Name,
    string Slug,
    string? LogoUrl,
    string? Industry,
    string? Size,
    Guid PlanId,
    string PlanName,
    bool IsActive,
    DateTime CreatedAt
);

public record CompanySummaryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Industry,
    string PlanName,
    bool IsActive,
    int UserCount,
    int JobCount,
    DateTime CreatedAt
);