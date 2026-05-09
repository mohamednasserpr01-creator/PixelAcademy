using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.Commands.Enrollments;
using PixelAcademy.Application.DTOs.Enrollments;
using PixelAcademy.Application.Queries.Enrollments;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public EnrollmentsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<EnrollmentDto>>> GetMyEnrollments(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetMyEnrollmentsQuery
        {
            StudentId = _currentUserService.UserId!.Value,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EnrollmentDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetEnrollmentByIdQuery { Id = id });
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<EnrollmentDto>> Create([FromBody] CreateEnrollmentRequestDto request)
    {
        var result = await _mediator.Send(new CreateEnrollmentCommand
        {
            StudentId = _currentUserService.UserId!.Value,
            CourseId = request.CourseId
        });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        await _mediator.Send(new UpdateEnrollmentStatusCommand
        {
            EnrollmentId = id,
            Status = request.Status
        });
        return NoContent();
    }

    [HttpPost("{id:guid}/rate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Rate(Guid id, [FromBody] RateRequest request)
    {
        await _mediator.Send(new RateCourseCommand
        {
            EnrollmentId = id,
            Rating = request.Rating,
            Review = request.Review
        });
        return NoContent();
    }
}

public class UpdateStatusRequest
{
    public EnrollmentStatus Status { get; set; }
}

public class RateRequest
{
    public int Rating { get; set; }
    public string? Review { get; set; }
}
