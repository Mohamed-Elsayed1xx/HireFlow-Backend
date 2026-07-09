using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Queries.GetApplicationById;

public record GetApplicationByIdQuery(Guid Id) : IRequest<Result<JobApplicationDto>>;
