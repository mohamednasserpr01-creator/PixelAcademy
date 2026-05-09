using AutoMapper;
using MediatR;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Transactions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Transactions;

public class GetMyTransactionsQueryHandler : IRequestHandler<GetMyTransactionsQuery, PagedResponse<TransactionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyTransactionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<TransactionDto>> Handle(GetMyTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _unitOfWork.Transactions.GetByUserAsync(request.UserId, cancellationToken);
        var query = transactions.AsQueryable();

        var totalCount = query.Count();
        var items = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResponse<TransactionDto>
        {
            Items = _mapper.Map<IReadOnlyList<TransactionDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
