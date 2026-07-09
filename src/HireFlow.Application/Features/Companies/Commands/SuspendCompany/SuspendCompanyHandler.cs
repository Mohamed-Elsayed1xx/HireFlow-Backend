using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.SuspendCompany;

public class SuspendCompanyHandler : IRequestHandler<SuspendCompanyCommand, Result>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IAuditService _auditService;

    public SuspendCompanyHandler(ICompanyRepository companyRepository, IAuditService auditService)
    {
        _companyRepository = companyRepository;
        _auditService = auditService;
    }

    public async Task<Result> Handle(SuspendCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id);

        if (company is null)
            return Result.Fail("NOT_FOUND", "Company not found.");

        if (!company.IsActive)
            return Result.Fail("ALREADY_SUSPENDED", "Company is already suspended.");

        company.IsActive = false;
        company.UpdatedAt = DateTime.UtcNow;

        await _companyRepository.UpdateAsync(company);

        await _auditService.LogAsync("CompanySuspended", "Company", company.Id);

        return Result.Ok("Company suspended successfully.");
    }
}
