using MediatR;
using HireFlow.Application.Common.Models;
using HireFlow.Application.Common.DTOs;

namespace HireFlow.Application.Features.Auth.Commands.RegisterCompany;

public record RegisterCompanyCommand(
    string CompanyName,
    string Industry,
    string Size,
    string AdminFirstName,
    string AdminLastName,
    string AdminEmail,
    string AdminPassword
) : IRequest<Result<LoginResponse>>;