using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace HireFlow.Application.Features.Companies.Queries.GetCompanyById;

public class GetCompanyByIdHandler : IRequestHandler<GetCompanyByIdQuery, Result<CompanyDto>>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompanyByIdHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<CompanyDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdWithDetailsAsync(request.Id);

        if (company is null)
            return Result<CompanyDto>.Fail("NOT_FOUND", "Company not found.");

        return Result<CompanyDto>.Ok(company.ToDto());
    }
}
