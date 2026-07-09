using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using MediatR;

namespace HireFlow.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery : IRequest<Result<List<NotificationDto>>>;
