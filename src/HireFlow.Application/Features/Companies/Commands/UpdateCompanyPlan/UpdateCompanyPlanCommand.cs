using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Companies.Commands.UpdateCompanyPlan;

public record UpdateCompanyPlanCommand(Guid CompanyId, Guid PlanId) : IRequest<Result>;
