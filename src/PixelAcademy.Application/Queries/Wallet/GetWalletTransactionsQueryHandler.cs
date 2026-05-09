using AutoMapper;
using MediatR;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Wallet;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Wallet;

public class GetWalletTransactionsQueryHandler : IRequestHandler<GetWalletTransactionsQuery, PagedResponse<WalletTransactionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetWalletTransactionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<WalletTransactionDto>> Handle(GetWalletTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _unitOfWork.WalletTransactions.GetByUserAsync(request.UserId, cancellationToken);
        var query = transactions.AsQueryable();

        var totalCount = query.Count();
        var items = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResponse<WalletTransactionDto>
        {
            Items = _mapper.Map<IReadOnlyList<WalletTransactionDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
