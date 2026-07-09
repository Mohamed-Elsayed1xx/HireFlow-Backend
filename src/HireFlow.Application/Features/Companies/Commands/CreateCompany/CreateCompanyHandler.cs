using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Entities;
using HireFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPlanRepository _planRepository;

    public CreateCompanyHandler(ICompanyRepository companyRepository, IPlanRepository planRepository)
    {
        _companyRepository = companyRepository;
        _planRepository = planRepository;
    }

    public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetByIdAsync(request.PlanId);

        if (plan is null)
            return Result<CompanyDto>.Fail("PLAN_NOT_FOUND", "Selected plan does not exist.");

        var slug = await GenerateUniqueSlugAsync(request.Name);

        var company = new Company
        {
            Name = request.Name,
            Slug = slug,
            Industry = request.Industry,
            Size = request.Size,
            PlanId = request.PlanId,
            IsActive = true
        };

        await _companyRepository.AddAsync(company);

        company.Plan = plan;

        return Result<CompanyDto>.Ok(company.ToDto(), "Company created successfully.");
    }

    private async Task<string> GenerateUniqueSlugAsync(string name)
    {
        var baseSlug = Slugify(name);
        var slug = baseSlug;
        var suffix = 1;

        while (await _companyRepository.SlugExistsAsync(slug))
        {
            slug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return slug;
    }

    private static string Slugify(string value)
    {
        var lowered = value.Trim().ToLowerInvariant();
        var slug = System.Text.RegularExpressions.Regex.Replace(lowered, @"[^a-z0-9\s-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
        return slug.Trim('-');
    }
}
