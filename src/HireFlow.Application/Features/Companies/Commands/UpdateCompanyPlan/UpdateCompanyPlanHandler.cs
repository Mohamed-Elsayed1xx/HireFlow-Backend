using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.UpdateCompanyPlan;

public class UpdateCompanyPlanHandler : IRequestHandler<UpdateCompanyPlanCommand, Result>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPlanRepository _planRepository;
    private readonly IAuditService _auditService;

    public UpdateCompanyPlanHandler(ICompanyRepository companyRepository, IPlanRepository planRepository, IAuditService auditService)
    {
        _companyRepository = companyRepository;
        _planRepository = planRepository;
        _auditService = auditService;
    }

    public async Task<Result> Handle(UpdateCompanyPlanCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.CompanyId);

        if (company is null)
            return Result.Fail("NOT_FOUND", "Company not found.");

        var plan = await _planRepository.GetByIdAsync(request.PlanId);

        if (plan is null)
            return Result.Fail("PLAN_NOT_FOUND", "Selected plan does not exist.");

        var oldPlanId = company.PlanId;
        company.PlanId = request.PlanId;
        company.UpdatedAt = DateTime.UtcNow;

        await _companyRepository.UpdateAsync(company);

        await _auditService.LogAsync("CompanyPlanChanged", "Company", company.Id, new { PlanId = oldPlanId }, new { PlanId = request.PlanId });

        return Result.Ok("Company plan updated successfully.");
    }
}
