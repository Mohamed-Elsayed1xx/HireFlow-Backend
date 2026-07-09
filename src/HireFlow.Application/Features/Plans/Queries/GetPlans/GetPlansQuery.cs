using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Plans.Queries.GetPlans;

public record GetPlansQuery : IRequest<Result<List<PlanDto>>>;
