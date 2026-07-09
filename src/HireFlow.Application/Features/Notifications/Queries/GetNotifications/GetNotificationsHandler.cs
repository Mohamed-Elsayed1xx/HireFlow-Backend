using HireFlow.Application.Common.DTOs;
using HireFlow.Application.Common.Models;
using HireFlow.Domain.Interfaces.Repositories;
using HireFlow.Domain.Interfaces.Services;
using MediatR;

namespace HireFlow.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, Result<List<NotificationDto>>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICurrentUserService _currentUser;

    public GetNotificationsHandler(
        INotificationRepository notificationRepository,
        ICurrentUserService currentUser)
    {
        _notificationRepository = notificationRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<List<NotificationDto>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository
            .GetByUserIdAsync(_currentUser.UserId);

        var result = notifications.Select(n => new NotificationDto(
    n.Id,
    n.Title,
    n.Body,
    n.Type,
    n.IsRead,
    n.Payload,
    n.CreatedAt
)).ToList();

        return Result<List<NotificationDto>>.Ok(result);
    }
}