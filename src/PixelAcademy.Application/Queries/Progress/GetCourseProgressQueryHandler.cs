using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Progress;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Progress;

public class GetCourseProgressQueryHandler : IRequestHandler<GetCourseProgressQuery, List<VideoProgressDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCourseProgressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<VideoProgressDto>> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
    {
        var progress = await _unitOfWork.VideoProgresses.GetByStudentAndCourseAsync(
            request.StudentId, request.CourseId, cancellationToken);
        return _mapper.Map<List<VideoProgressDto>>(progress);
    }
}
