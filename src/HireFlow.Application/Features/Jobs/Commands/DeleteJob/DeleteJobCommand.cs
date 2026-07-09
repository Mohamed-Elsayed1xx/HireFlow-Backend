using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Jobs.Commands.DeleteJob;

public record DeleteJobCommand(Guid Id) : IRequest<Result>;