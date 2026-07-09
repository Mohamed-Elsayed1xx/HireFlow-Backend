using HireFlow.Application.Common.DTOs;
using HireFlow.Domain.Entities;

namespace HireFlow.Application.Common.Mappings;

public static class JobMappings
{
    public static JobDto ToDto(this Job job) => new(
        job.Id,
        job.CompanyId,
        job.CreatedById,
        job.Title,
        job.Department,
        job.Location,
        job.Type.ToString(),
        job.ExperienceLevel?.ToString(),
        job.SalaryMin,
        job.SalaryMax,
        job.Description,
        job.Requirements,
        job.Status.ToString(),
        job.ClosingDate,
        job.CreatedAt
    );

    public static PublicJobDto ToPublicDto(this Job job) => new(
        job.Id,
        job.Company.Name,
        job.Title,
        job.Department,
        job.Location,
        job.Type.ToString(),
        job.ExperienceLevel?.ToString(),
        job.SalaryMin,
        job.SalaryMax,
        job.Description,
        job.Requirements,
        job.ClosingDate,
        job.CreatedAt
    );
}
