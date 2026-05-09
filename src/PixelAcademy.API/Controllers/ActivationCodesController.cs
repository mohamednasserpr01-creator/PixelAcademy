using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.Commands.ActivationCodes;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Application.Queries.ActivationCodes;
using PixelAcademy.Application.Queries.Transactions;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
public class ActivationCodesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ActivationCodesController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(typeof(ActivationCodeDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ActivationCodeDto>> Generate([FromBody] GenerateActivationCodeRequestDto request)
    {
        var result = await _mediator.Send(new GenerateActivationCodeCommand
        {
            Type = request.Type,
            Value = request.Value,
            CourseId = request.CourseId,
            LectureId = request.LectureId,
            MaxRedemptions = request.MaxRedemptions,
            ExpiresAt = request.ExpiresAt,
            GeneratedById = _currentUserService.UserId!.Value
        });
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("redeem")]
    [Authorize(Roles = "Student,Instructor,Admin")]
    [ProducesResponseType(typeof(RedeemResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RedeemResultDto>> Redeem([FromBody] RedeemActivationCodeRequestDto request)
    {
        var result = await _mediator.Send(new RedeemActivationCodeCommand
        {
            Code = request.Code,
            StudentId = _currentUserService.UserId!.Value
        });
        return Ok(result);
    }

    [HttpPost("{id:guid}/disable")]
    [Authorize(Roles = "Instructor,Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Disable(Guid id)
    {
        await _mediator.Send(new DisableActivationCodeCommand
        {
            Id = id,
            RequestedById = _currentUserService.UserId!.Value
        });
        return NoContent();
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PagedResponse<ActivationCodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ActivationCodeDto>>> GetMyCodes(
        [FromQuery] bool asGenerator = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetMyActivationCodesQuery
        {
            UserId = _currentUserService.UserId!.Value,
            AsGenerator = asGenerator,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ActivationCodeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ActivationCodeDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetActivationCodeByIdQuery { Id = id });
        return Ok(result);
    }
}
