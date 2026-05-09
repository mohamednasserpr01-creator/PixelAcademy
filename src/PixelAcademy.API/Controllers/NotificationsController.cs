using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Notifications;
using PixelAcademy.Application.DTOs.Notifications;
using PixelAcademy.Application.Queries.Notifications;
using Asp.Versioning;
using System.Security.Claims;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new GetNotificationsQuery(userId, page, pageSize));
        return Ok(result);
    }

    [HttpGet("unread")]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetUnreadNotifications()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new GetUnreadNotificationsQuery(userId));
        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new GetUnreadNotificationCountQuery(userId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NotificationDto>> GetNotification(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new GetNotificationByIdQuery(id, userId));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationRequestDto request)
    {
        var command = new CreateNotificationCommand(
            request.UserId,
            request.Title,
            request.Message,
            request.Type,
            request.ActionUrl,
            request.RelatedEntityId,
            request.RelatedEntityType
        );
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetNotification), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/mark-read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new MarkNotificationAsReadCommand(id, userId));
        return NoContent();
    }

    [HttpPost("mark-all-read")]
    public async Task<ActionResult<int>> MarkAllAsRead()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var count = await _mediator.Send(new MarkAllNotificationsAsReadCommand(userId));
        return Ok(count);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new DeleteNotificationCommand(id, userId));
        return NoContent();
    }
}
