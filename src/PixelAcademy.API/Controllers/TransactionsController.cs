using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Transactions;
using PixelAcademy.Application.Queries.Transactions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public TransactionsController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TransactionDto>>> GetMyTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetMyTransactionsQuery
        {
            UserId = _currentUserService.UserId!.Value,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return Ok(result);
    }
}
