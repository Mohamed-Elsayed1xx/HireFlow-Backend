using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.JobApplications.Commands.RejectApplication;

public record RejectApplicationCommand(Guid ApplicationId, string? Note) : IRequest<Result>;
