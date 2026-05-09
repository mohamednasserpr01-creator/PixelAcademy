using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Wallet;

namespace PixelAcademy.Application.Queries.Wallet;

public class GetWalletBalanceQuery : IQuery<WalletBalanceDto>
{
    public Guid UserId { get; set; }
}
