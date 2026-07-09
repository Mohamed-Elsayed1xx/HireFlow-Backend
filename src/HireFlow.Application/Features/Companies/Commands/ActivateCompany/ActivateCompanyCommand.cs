using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.ActivateCompany;

public record ActivateCompanyCommand(Guid Id) : IRequest<Result>;
