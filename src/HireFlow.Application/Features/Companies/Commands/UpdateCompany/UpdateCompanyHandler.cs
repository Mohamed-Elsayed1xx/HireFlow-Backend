using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.UpdateCompany;

public class UpdateCompanyHandler : IRequestHandler<UpdateCompanyCommand, Result<CompanyDto>>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompanyDto>> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdWithDetailsAsync(request.Id);

        if (company is null)
            return Result<CompanyDto>.Fail("NOT_FOUND", "Company not found.");

        company.Name = request.Name;
        company.Industry = request.Industry;
        company.Size = request.Size;
        company.UpdatedAt = DateTime.UtcNow;

        await _companyRepository.UpdateAsync(company);

        return Result<CompanyDto>.Ok(company.ToDto(), "Company updated successfully.");
    }
}
