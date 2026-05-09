using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Media;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Media;

public class GetMediaByCourseQueryHandler : IRequestHandler<GetMediaByCourseQuery, List<MediaAssetDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMediaByCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<MediaAssetDto>> Handle(GetMediaByCourseQuery request, CancellationToken cancellationToken)
    {
        var media = await _unitOfWork.MediaAssets.GetByCourseAsync(request.CourseId, cancellationToken);
        return _mapper.Map<List<MediaAssetDto>>(media);
    }
}
