using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Analytics.Queries.GetKpis;

public record GetKpisQuery : IRequest<Result<KpisDto>>;
