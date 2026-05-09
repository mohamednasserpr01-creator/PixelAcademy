using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.ContentItems;
using PixelAcademy.Application.DTOs.ContentItems;
using PixelAcademy.Application.Queries.ContentItems;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/courses/{courseId:guid}/lectures/{lectureId:guid}/[controller]")]
[Produces("application/json")]
public class ContentItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ContentItemsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ContentItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContentItemDto>>> GetByLecture(Guid courseId, Guid lectureId)
    {
        var result = await _mediator.Send(new GetContentItemsByLectureQuery { LectureId = lectureId });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ContentItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentItemDto>> GetById(Guid courseId, Guid lectureId, Guid id)
    {
        var result = await _mediator.Send(new GetContentItemByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(ContentItemDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ContentItemDto>> Create(Guid courseId, Guid lectureId, [FromBody] CreateContentItemRequestDto request)
    {
        var result = await _mediator.Send(new CreateContentItemCommand
        {
            LectureId = lectureId,
            Title = request.Title,
            Description = request.Description,
            OrderIndex = request.OrderIndex,
            Type = request.Type,
            IsRequired = request.IsRequired,
            DurationSeconds = request.DurationSeconds,
            ExternalUrl = request.ExternalUrl,
            MediaAssetId = request.MediaAssetId
        });
        return CreatedAtAction(nameof(GetById), new { courseId, lectureId, id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(ContentItemDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContentItemDto>> Update(Guid courseId, Guid lectureId, Guid id, [FromBody] UpdateContentItemRequestDto request)
    {
        var result = await _mediator.Send(new UpdateContentItemCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            OrderIndex = request.OrderIndex,
            Type = request.Type,
            IsRequired = request.IsRequired,
            DurationSeconds = request.DurationSeconds,
            ExternalUrl = request.ExternalUrl,
            MediaAssetId = request.MediaAssetId
        });
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid courseId, Guid lectureId, Guid id)
    {
        await _mediator.Send(new DeleteContentItemCommand { Id = id });
        return NoContent();
    }
}
