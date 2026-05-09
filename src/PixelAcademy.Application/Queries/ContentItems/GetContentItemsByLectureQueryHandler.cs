using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.ContentItems;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.ContentItems;

public class GetContentItemsByLectureQueryHandler : IRequestHandler<GetContentItemsByLectureQuery, List<ContentItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetContentItemsByLectureQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ContentItemDto>> Handle(GetContentItemsByLectureQuery request, CancellationToken cancellationToken)
    {
        var items = await _unitOfWork.ContentItems.GetByLectureAsync(request.LectureId, cancellationToken);
        return _mapper.Map<List<ContentItemDto>>(items);
    }
}
