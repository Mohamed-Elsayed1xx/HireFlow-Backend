using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.SuspendCompany;

public record SuspendCompanyCommand(Guid Id) : IRequest<Result>;
