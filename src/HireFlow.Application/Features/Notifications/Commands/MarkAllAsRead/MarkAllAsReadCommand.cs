using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Notifications.Commands.MarkAllAsRead;

public record MarkAllAsReadCommand : IRequest<Result>;
