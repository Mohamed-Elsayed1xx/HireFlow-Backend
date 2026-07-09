using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Analytics.Queries.GetHiringFunnel;

public record GetHiringFunnelQuery : IRequest<Result<List<HiringFunnelDto>>>;
