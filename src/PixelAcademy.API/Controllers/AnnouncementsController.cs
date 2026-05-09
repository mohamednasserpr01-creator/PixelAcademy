using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Announcements;
using PixelAcademy.Application.DTOs.Announcements;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Application.Queries.Announcements;
using Asp.Versioning;
using System.Security.Claims;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
public class AnnouncementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnnouncementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<AnnouncementDto>>> GetAnnouncements([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetAnnouncementsQuery(page, pageSize));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AnnouncementDto>> GetAnnouncement(Guid id)
    {
        var result = await _mediator.Send(new GetAnnouncementByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("course/{courseId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<AnnouncementDto>>> GetCourseAnnouncements(Guid courseId)
    {
        var result = await _mediator.Send(new GetCourseAnnouncementsQuery(courseId));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<AnnouncementDto>> CreateAnnouncement(CreateAnnouncementRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateAnnouncementCommand(
            request.Title,
            request.Content,
            request.Target,
            request.CourseId,
            request.ScheduledPublishAt,
            request.Priority,
            userId
        );
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAnnouncement), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<AnnouncementDto>> UpdateAnnouncement(Guid id, UpdateAnnouncementRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new UpdateAnnouncementCommand(id, request.Title, request.Content, request.ScheduledPublishAt, request.Priority, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> DeleteAnnouncement(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new DeleteAnnouncementCommand(id, userId));
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<AnnouncementDto>> PublishAnnouncement(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new PublishAnnouncementCommand(id, userId));
        return Ok(result);
    }
}
