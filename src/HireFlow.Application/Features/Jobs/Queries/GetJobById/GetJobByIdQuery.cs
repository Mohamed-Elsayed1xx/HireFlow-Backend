using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Queries.GetJobById;

public record GetJobByIdQuery(Guid Id) : IRequest<Result<JobDto>>;