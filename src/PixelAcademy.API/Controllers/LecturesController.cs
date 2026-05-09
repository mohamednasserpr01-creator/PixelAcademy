using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Lectures;
using PixelAcademy.Application.DTOs.Lectures;
using PixelAcademy.Application.Queries.Lectures;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/courses/{courseId:guid}/[controller]")]
[Produces("application/json")]
public class LecturesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public LecturesController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<LectureDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LectureDto>>> GetByCourse(Guid courseId)
    {
        var result = await _mediator.Send(new GetLecturesByCourseQuery
        {
            CourseId = courseId,
            StudentId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : null
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LectureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LectureDto>> GetById(Guid courseId, Guid id)
    {
        var result = await _mediator.Send(new GetLectureByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(LectureDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<LectureDto>> Create(Guid courseId, [FromBody] CreateLectureRequestDto request)
    {
        var result = await _mediator.Send(new CreateLectureCommand
        {
            CourseId = courseId,
            Title = request.Title,
            Description = request.Description,
            OrderIndex = request.OrderIndex,
            DurationMinutes = request.DurationMinutes,
            IsPreview = request.IsPreview,
            VideoUrl = request.VideoUrl
        });
        return CreatedAtAction(nameof(GetById), new { courseId, id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(LectureDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<LectureDto>> Update(Guid courseId, Guid id, [FromBody] UpdateLectureRequestDto request)
    {
        var result = await _mediator.Send(new UpdateLectureCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            OrderIndex = request.OrderIndex,
            DurationMinutes = request.DurationMinutes,
            IsPreview = request.IsPreview,
            VideoUrl = request.VideoUrl
        });
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid courseId, Guid id)
    {
        await _mediator.Send(new DeleteLectureCommand { Id = id });
        return NoContent();
    }
}
