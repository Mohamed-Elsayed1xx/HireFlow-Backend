using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Queries.GetMyApplications;

public record GetMyApplicationsQuery : IRequest<Result<List<JobApplicationDto>>>;
