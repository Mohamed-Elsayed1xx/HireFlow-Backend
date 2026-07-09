using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(
    string Name,
    string? Industry,
    string? Size,
    Guid PlanId
) : IRequest<Result<CompanyDto>>;
