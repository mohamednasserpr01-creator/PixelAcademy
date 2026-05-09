using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Progress;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.WatchSessions;

public class GetCompletedLecturesQueryHandler : IRequestHandler<GetCompletedLecturesQuery, List<LectureProgressSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCompletedLecturesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<LectureProgressSummaryDto>> Handle(GetCompletedLecturesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.VideoProgress> progresses;
        if (request.CourseId.HasValue && request.CourseId.Value != Guid.Empty)
        {
            progresses = await _unitOfWork.VideoProgresses.GetByStudentAndCourseAsync(request.StudentId, request.CourseId.Value, cancellationToken);
        }
        else
        {
            var allProgresses = await _unitOfWork.VideoProgresses.GetAllAsync(cancellationToken);
            progresses = allProgresses.Where(p => p.StudentId == request.StudentId).ToList();
        }
        var completed = progresses.Where(p => p.IsCompleted).ToList();
        return _mapper.Map<List<LectureProgressSummaryDto>>(completed);
    }
}
