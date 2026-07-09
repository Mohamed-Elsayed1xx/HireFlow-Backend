using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.ActivateCompany;

public class ActivateCompanyHandler : IRequestHandler<ActivateCompanyCommand, Result>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAuditService _auditService;

    public ActivateCompanyHandler(ICompanyRepository companyRepository, IAuditService auditService)
    {
        _companyRepository = companyRepository;
        _auditService = auditService;
    }

    public async Task<Result> Handle(ActivateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id);

        if (company is null)
            return Result.Fail("NOT_FOUND", "Company not found.");

        if (company.IsActive)
            return Result.Fail("ALREADY_ACTIVE", "Company is already active.");

        company.IsActive = true;
        company.UpdatedAt = DateTime.UtcNow;

        await _companyRepository.UpdateAsync(company);

        await _auditService.LogAsync("CompanyActivated", "Company", company.Id);

        return Result.Ok("Company activated successfully.");
    }
}
