using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.WatchSessions;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Application.Queries.WatchSessions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class WatchSessionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public WatchSessionsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpPost("start")]
    [ProducesResponseType(typeof(WatchSessionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<WatchSessionDto>> Start([FromBody] StartWatchSessionRequestDto request)
    {
        var result = await _mediator.Send(new StartWatchSessionCommand
        {
            StudentId = _currentUserService.UserId!.Value,
            LectureId = request.LectureId,
            CourseId = request.CourseId
        });
        return CreatedAtAction(nameof(GetContinueWatching), new { }, result);
    }

    [HttpPost("update-progress")]
    [ProducesResponseType(typeof(WatchSessionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WatchSessionDto>> UpdateProgress([FromBody] UpdateWatchProgressRequestDto request)
    {
        var result = await _mediator.Send(new UpdateWatchProgressCommand
        {
            StudentId = _currentUserService.UserId!.Value,
            LectureId = request.LectureId,
            CurrentPositionSeconds = request.CurrentPositionSeconds,
            DurationWatchedSeconds = request.DurationWatchedSeconds
        });
        return Ok(result);
    }

    [HttpPost("finish")]
    [ProducesResponseType(typeof(WatchSessionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WatchSessionDto>> Finish([FromBody] FinishWatchSessionRequestDto request)
    {
        var result = await _mediator.Send(new FinishWatchSessionCommand
        {
            StudentId = _currentUserService.UserId!.Value,
            LectureId = request.LectureId,
            FinalPositionSeconds = request.FinalPositionSeconds,
            TotalDurationWatchedSeconds = request.TotalDurationWatchedSeconds
        });
        return Ok(result);
    }

    [HttpPost("signed-url/{lectureId:guid}")]
    [ProducesResponseType(typeof(SignedVideoUrlDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<SignedVideoUrlDto>> GetSignedUrl(Guid lectureId)
    {
        var result = await _mediator.Send(new GenerateSignedVideoUrlCommand
        {
            StudentId = _currentUserService.UserId!.Value,
            LectureId = lectureId
        });
        return Ok(result);
    }

    [HttpGet("continue")]
    [ProducesResponseType(typeof(ContinueWatchingDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContinueWatchingDto>> GetContinueWatching()
    {
        var result = await _mediator.Send(new GetContinueWatchingQuery
        {
            StudentId = _currentUserService.UserId!.Value
        });
        return result == null ? Ok(null) : Ok(result);
    }

    [HttpGet("completed")]
    [ProducesResponseType(typeof(List<PixelAcademy.Application.DTOs.Progress.LectureProgressSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PixelAcademy.Application.DTOs.Progress.LectureProgressSummaryDto>>> GetCompletedLectures([FromQuery] Guid? courseId)
    {
        var result = await _mediator.Send(new GetCompletedLecturesQuery
        {
            StudentId = _currentUserService.UserId!.Value,
            CourseId = courseId
        });
        return Ok(result);
    }

    [HttpGet("course-progress/{courseId:guid}")]
    [ProducesResponseType(typeof(PixelAcademy.Application.DTOs.Progress.CourseProgressDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PixelAcademy.Application.DTOs.Progress.CourseProgressDto>> GetCourseProgress(Guid courseId)
    {
        var result = await _mediator.Send(new GetCourseProgressQuery
        {
            StudentId = _currentUserService.UserId!.Value,
            CourseId = courseId
        });
        return Ok(result);
    }
}
