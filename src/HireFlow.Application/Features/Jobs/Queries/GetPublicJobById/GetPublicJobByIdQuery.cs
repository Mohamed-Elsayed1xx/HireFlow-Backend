using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Queries.GetPublicJobById;

public record GetPublicJobByIdQuery(Guid Id) : IRequest<Result<PublicJobDto>>;
