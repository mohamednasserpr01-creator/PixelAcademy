using MediatR;
using PixelAcademy.Application.DTOs.Wallet;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Wallet;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, WalletBalanceDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWalletBalanceQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<WalletBalanceDto> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null) throw new NotFoundException("User", request.UserId);

        return new WalletBalanceDto
        {
            UserId = user.Id,
            Balance = user.WalletBalance
        };
    }
}
