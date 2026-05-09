using MediatR;
using PixelAcademy.Application.DTOs.Analytics;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Analytics;

public class GetLectureAnalyticsQueryHandler : IRequestHandler<GetLectureAnalyticsQuery, LectureAnalyticsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetLectureAnalyticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LectureAnalyticsDto> Handle(GetLectureAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        var sessions = await _unitOfWork.WatchSessions.GetByCourseAsync(lecture.CourseId, cancellationToken);
        var lectureSessions = sessions.Where(s => s.LectureId == request.LectureId).ToList();
        var progresses = await _unitOfWork.VideoProgresses.GetByCourseAsync(lecture.CourseId, cancellationToken);
        var lectureProgresses = progresses.Where(p => p.LectureId == request.LectureId).ToList();

        var totalWatchTime = lectureSessions.Sum(s => s.DurationWatchedSeconds);
        var uniqueWatchers = lectureSessions.Select(s => s.StudentId).Distinct().Count();
        var completionCount = lectureProgresses.Count(p => p.IsCompleted);
        var dropOffCount = lectureSessions.Count(s => !s.IsCompleted);
        var avgCompletion = lectureProgresses.Any()
            ? lectureProgresses.Average(p => p.CompletionPercent)
            : 0;

        return new LectureAnalyticsDto
        {
            LectureId = lecture.Id,
            LectureTitle = lecture.Title,
            TotalWatchTimeSeconds = totalWatchTime,
            UniqueWatchers = uniqueWatchers,
            CompletionCount = completionCount,
            DropOffCount = dropOffCount,
            AverageCompletionPercent = avgCompletion
        };
    }
}
