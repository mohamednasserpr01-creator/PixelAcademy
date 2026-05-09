using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Admin;
using PixelAcademy.Application.DTOs.Admin;
using PixelAcademy.Application.DTOs.AuditLogs;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Application.Queries.Admin;
using PixelAcademy.Domain.Enums;
using Asp.Versioning;
using System.Security.Claims;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
    {
        var result = await _mediator.Send(new GetAdminDashboardQuery());
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/ban")]
    public async Task<IActionResult> BanUser(Guid userId, [FromBody] BanUserRequestDto request)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new BanUserCommand(userId, request.Reason, adminId));
        return NoContent();
    }

    [HttpPost("users/{userId:guid}/unban")]
    public async Task<IActionResult> UnbanUser(Guid userId)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new UnbanUserCommand(userId, adminId));
        return NoContent();
    }

    [HttpPost("courses/{courseId:guid}/disable")]
    public async Task<IActionResult> DisableCourse(Guid courseId)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new DisableCourseCommand(courseId, adminId));
        return NoContent();
    }

    [HttpPost("courses/{courseId:guid}/enable")]
    public async Task<IActionResult> EnableCourse(Guid courseId)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new EnableCourseCommand(courseId, adminId));
        return NoContent();
    }

    [HttpPost("lectures/{lectureId:guid}/disable")]
    public async Task<IActionResult> DisableLecture(Guid lectureId)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new DisableLectureCommand(lectureId, adminId));
        return NoContent();
    }

    [HttpPost("lectures/{lectureId:guid}/enable")]
    public async Task<IActionResult> EnableLecture(Guid lectureId)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new EnableLectureCommand(lectureId, adminId));
        return NoContent();
    }
}

public class BanUserRequestDto
{
    public string Reason { get; set; } = string.Empty;
}
