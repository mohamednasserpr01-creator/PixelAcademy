using AutoMapper;
using MediatR;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.ActivationCodes;

public class GetMyActivationCodesQueryHandler : IRequestHandler<GetMyActivationCodesQuery, PagedResponse<ActivationCodeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyActivationCodesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ActivationCodeDto>> Handle(GetMyActivationCodesQuery request, CancellationToken cancellationToken)
    {
        var all = await _unitOfWork.ActivationCodes.GetAllAsync(cancellationToken);
        var query = all.AsQueryable();

        if (request.AsGenerator)
            query = query.Where(c => c.GeneratedById == request.UserId);
        else
            query = query.Where(c => c.LastRedeemedById == request.UserId);

        var totalCount = query.Count();
        var items = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResponse<ActivationCodeDto>
        {
            Items = _mapper.Map<IReadOnlyList<ActivationCodeDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
