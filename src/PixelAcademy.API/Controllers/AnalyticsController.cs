using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.DTOs.Analytics;
using PixelAcademy.Application.Queries.Analytics;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Instructor,Admin")]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("lecture/{lectureId:guid}")]
    [ProducesResponseType(typeof(LectureAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LectureAnalyticsDto>> GetLectureAnalytics(Guid lectureId)
    {
        var result = await _mediator.Send(new GetLectureAnalyticsQuery { LectureId = lectureId });
        return Ok(result);
    }

    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(CourseAnalyticsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CourseAnalyticsDto>> GetCourseAnalytics(Guid courseId)
    {
        var result = await _mediator.Send(new GetCourseAnalyticsQuery { CourseId = courseId });
        return Ok(result);
    }
}
