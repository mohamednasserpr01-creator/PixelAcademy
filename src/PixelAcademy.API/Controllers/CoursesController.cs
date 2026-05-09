using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.Commands.Courses;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Application.Queries.Courses;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public CoursesController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<CourseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CourseDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null,
        [FromQuery] string? level = null)
    {
        var result = await _mediator.Send(new GetAllCoursesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            Category = category,
            Level = level
        });
        return Ok(result);
    }

    [HttpGet("published")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResponse<CourseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CourseDto>>> GetPublished(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? category = null)
    {
        var result = await _mediator.Send(new GetPublishedCoursesQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            Category = category
        });
        return Ok(result);
    }

    [HttpGet("my-courses")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(PagedResponse<CourseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CourseDto>>> GetMyCourses(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetMyCoursesQuery
        {
            InstructorId = _currentUserService.UserId!.Value,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDetailDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery
        {
            Id = id,
            CurrentUserId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : null
        });
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseRequestDto request)
    {
        var result = await _mediator.Send(new CreateCourseCommand
        {
            Title = request.Title,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            Level = request.Level,
            Price = request.Price,
            Category = request.Category,
            Tags = request.Tags,
            InstructorId = _currentUserService.UserId!.Value
        });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> Update(Guid id, [FromBody] UpdateCourseRequestDto request)
    {
        var result = await _mediator.Send(new UpdateCourseCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            Level = request.Level,
            Status = request.Status,
            Price = request.Price,
            Category = request.Category,
            Tags = request.Tags
        });
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteCourseCommand
        {
            Id = id,
            RequestedById = _currentUserService.UserId!.Value
        });
        return NoContent();
    }
}
