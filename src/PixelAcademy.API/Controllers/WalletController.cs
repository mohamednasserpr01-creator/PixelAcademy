using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Wallet;
using PixelAcademy.Application.Queries.Wallet;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public WalletController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("balance")]
    [ProducesResponseType(typeof(WalletBalanceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WalletBalanceDto>> GetBalance()
    {
        var result = await _mediator.Send(new GetWalletBalanceQuery
        {
            UserId = _currentUserService.UserId!.Value
        });
        return Ok(result);
    }

    [HttpGet("transactions")]
    [ProducesResponseType(typeof(PagedResponse<WalletTransactionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<WalletTransactionDto>>> GetTransactions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetWalletTransactionsQuery
        {
            UserId = _currentUserService.UserId!.Value,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        return Ok(result);
    }
}
