using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Interviews.Commands.CancelInterview;

public record CancelInterviewCommand(Guid Id) : IRequest<Result>;
