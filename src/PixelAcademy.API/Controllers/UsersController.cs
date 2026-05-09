using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.Commands.Users;
using PixelAcademy.Application.DTOs.Auth;
using PixelAcademy.Application.DTOs.Users;
using PixelAcademy.Application.Queries.Users;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public UsersController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(PagedResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<UserDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? role = null)
    {
        var result = await _mediator.Send(new GetAllUsersQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            Role = role
        });
        return Ok(result);
    }

    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var result = await _mediator.Send(new GetUserProfileQuery
        {
            UserId = _currentUserService.UserId!.Value
        });
        return Ok(result);
    }

    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileRequestDto request)
    {
        var result = await _mediator.Send(new UpdateProfileCommand
        {
            UserId = _currentUserService.UserId!.Value,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Bio = request.Bio,
            AvatarUrl = request.AvatarUrl
        });
        return Ok(result);
    }

    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        await _mediator.Send(new ChangePasswordCommand
        {
            UserId = _currentUserService.UserId!.Value,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        });
        return NoContent();
    }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
