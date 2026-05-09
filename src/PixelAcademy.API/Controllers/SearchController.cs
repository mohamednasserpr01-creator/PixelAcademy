using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Application.DTOs.Search;
using PixelAcademy.Application.Queries.Search;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[AllowAnonymous]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<SearchResultDto>> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new SearchQuery(q, page, pageSize));
        return Ok(result);
    }

    [HttpGet("courses")]
    public async Task<ActionResult<PagedResult<SearchResultDto>>> SearchCourses(
        [FromQuery] string? q,
        [FromQuery] string? category,
        [FromQuery] CourseLevel? level,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new SearchCoursesQuery(q, category, level, page, pageSize));
        return Ok(result);
    }
}
