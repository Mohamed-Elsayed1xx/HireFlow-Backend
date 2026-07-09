using HireFlow.Application.Features.Notifications.Commands.MarkAllAsRead;
using HireFlow.Application.Features.Notifications.Commands.MarkAsRead;
using HireFlow.Application.Features.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HireFlow.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var result = await _mediator.Send(new GetNotificationsQuery());
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _mediator.Send(new MarkAsReadCommand(id));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _mediator.Send(new MarkAllAsReadCommand());
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
