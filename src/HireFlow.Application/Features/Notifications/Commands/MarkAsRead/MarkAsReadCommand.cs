using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid NotificationId) : IRequest<Result>;
