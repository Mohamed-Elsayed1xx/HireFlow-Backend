using HireFlow.Application.Common.DTOs;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Common.Mappings;

public static class CompanyMappings
{
    public static CompanyDto ToDto(this Company company) => new(
        company.Id,
        company.Name,
        company.Slug,
        company.LogoUrl,
        company.Industry,
        company.Size,
        company.PlanId,
        company.Plan?.Name ?? string.Empty,
        company.IsActive,
        company.CreatedAt
    );

    public static CompanySummaryDto ToSummaryDto(this Company company) => new(
        company.Id,
        company.Name,
        company.Slug,
        company.Industry,
        company.Plan?.Name ?? string.Empty,
        company.IsActive,
        company.Users.Count,
        company.Jobs.Count,
        company.CreatedAt
    );
}
