using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.ContentItems;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.ContentItems;

public class GetContentItemByIdQueryHandler : IRequestHandler<GetContentItemByIdQuery, ContentItemDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContentItemByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ContentItemDto> Handle(GetContentItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.ContentItems.GetByIdAsync(request.Id, cancellationToken);
        if (item == null) throw new NotFoundException("ContentItem", request.Id);
        return _mapper.Map<ContentItemDto>(item);
    }
}
