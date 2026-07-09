using HireFlow.Application.Common.Mappings;
using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using MediatR;

namespace HireFlow.Application.Features.Companies.Queries.GetCompanies;

public class GetCompaniesHandler : IRequestHandler<GetCompaniesQuery, Result<PagedResult<CompanySummaryDto>>>
{
    private readonly ICompanyRepository _companyRepository;

    public GetCompaniesHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<Result<PagedResult<CompanySummaryDto>>> Handle(
        GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var companies = await _companyRepository.GetAllWithDetailsAsync();

        if (request.IsActive.HasValue)
            companies = companies.Where(c => c.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
            companies = companies.Where(c =>
                c.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                c.Slug.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

        var totalCount = companies.Count();
        var items = companies
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => c.ToSummaryDto());

        return Result<PagedResult<CompanySummaryDto>>.Ok(new PagedResult<CompanySummaryDto>
        {
            Data = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        });
    }
}
