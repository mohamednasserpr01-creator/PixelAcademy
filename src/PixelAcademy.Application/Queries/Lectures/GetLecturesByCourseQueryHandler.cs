using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Lectures;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Domain.Specifications;

namespace PixelAcademy.Application.Queries.Lectures;

public class GetLecturesByCourseQueryHandler : IRequestHandler<GetLecturesByCourseQuery, List<LectureDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLecturesByCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<LectureDto>> Handle(GetLecturesByCourseQuery request, CancellationToken cancellationToken)
    {
        var spec = new LecturesByCourseOrderedSpec(request.CourseId);
        var lectures = await _unitOfWork.Lectures.GetAsync(spec, cancellationToken);

        var dtos = _mapper.Map<List<LectureDto>>(lectures);

        if (request.StudentId.HasValue)
        {
            foreach (var dto in dtos)
            {
                var progress = await _unitOfWork.VideoProgresses.GetByStudentAndLectureAsync(
                    request.StudentId.Value, dto.Id, cancellationToken);
                if (progress != null)
                {
                    dto.IsWatched = progress.IsCompleted;
                    dto.WatchedSeconds = progress.WatchedSeconds;
                }
            }
        }

        return dtos;
    }
}
