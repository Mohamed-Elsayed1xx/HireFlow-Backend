using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Analytics.Queries.GetSourceBreakdown;

public record GetSourceBreakdownQuery : IRequest<Result<List<SourceBreakdownDto>>>;
