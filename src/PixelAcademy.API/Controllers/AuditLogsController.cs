using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.DTOs.AuditLogs;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Application.Queries.Admin;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] AuditActionType? actionType = null)
    {
        var result = await _mediator.Send(new GetAuditLogsQuery(page, pageSize, actionType));
        return Ok(result);
    }

    [HttpGet("admin-actions")]
    public async Task<ActionResult<PagedResult<AuditLogDto>>> GetAdminActions([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(new GetAdminAuditLogsQuery(page, pageSize));
        return Ok(result);
    }
}
