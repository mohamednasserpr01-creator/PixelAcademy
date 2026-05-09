using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Progress;
using PixelAcademy.Application.DTOs.Progress;
using PixelAcademy.Application.Queries.Progress;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ProgressController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(List<VideoProgressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VideoProgressDto>>> GetCourseProgress(Guid courseId)
    {
        var result = await _mediator.Send(new GetCourseProgressQuery
        {
            StudentId = _currentUserService.UserId!.Value,
            CourseId = courseId
        });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VideoProgressDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<VideoProgressDto>> UpdateProgress([FromBody] UpdateProgressRequestDto request)
    {
        var result = await _mediator.Send(new UpdateVideoProgressCommand
        {
            StudentId = _currentUserService.UserId!.Value,
            LectureId = request.LectureId,
            WatchedSeconds = request.WatchedSeconds,
            IsCompleted = request.IsCompleted
        });
        return Ok(result);
    }
}
