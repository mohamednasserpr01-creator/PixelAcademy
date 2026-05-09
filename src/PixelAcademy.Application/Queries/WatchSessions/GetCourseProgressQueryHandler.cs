using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Progress;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.WatchSessions;

public class GetCourseProgressQueryHandler : IRequestHandler<GetCourseProgressQuery, CourseProgressDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCourseProgressQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CourseProgressDto> Handle(GetCourseProgressQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.CourseId);

        var lectures = await _unitOfWork.Lectures.GetByCourseAsync(request.CourseId, cancellationToken);
        var progresses = await _unitOfWork.VideoProgresses.GetByStudentAndCourseAsync(request.StudentId, request.CourseId, cancellationToken);

        var progressDict = progresses.ToDictionary(p => p.LectureId);

        var lectureSummaries = lectures.Select(l =>
        {
            var progress = progressDict.ContainsKey(l.Id) ? progressDict[l.Id] : null;
            return new LectureProgressSummaryDto
            {
                LectureId = l.Id,
                LectureTitle = l.Title,
                DurationMinutes = l.DurationMinutes,
                CompletionPercent = progress?.CompletionPercent ?? 0,
                IsCompleted = progress?.IsCompleted ?? false,
                LastPositionSeconds = progress?.WatchedSeconds ?? 0
            };
        }).ToList();

        var completedCount = lectureSummaries.Count(l => l.IsCompleted);
        var overallPercent = lectures.Count > 0
            ? lectureSummaries.Sum(l => l.CompletionPercent) / lectures.Count
            : 0;

        return new CourseProgressDto
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            TotalLectures = lectures.Count,
            CompletedLectures = completedCount,
            OverallCompletionPercent = overallPercent,
            Lectures = lectureSummaries
        };
    }
}
