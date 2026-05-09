using MediatR;
using PixelAcademy.Application.DTOs.Analytics;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Analytics;

public class GetCourseAnalyticsQueryHandler : IRequestHandler<GetCourseAnalyticsQuery, CourseAnalyticsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCourseAnalyticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CourseAnalyticsDto> Handle(GetCourseAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.CourseId);

        var enrollments = await _unitOfWork.Enrollments.GetByCourseAsync(request.CourseId, cancellationToken);
        var sessions = await _unitOfWork.WatchSessions.GetByCourseAsync(request.CourseId, cancellationToken);
        var progresses = await _unitOfWork.VideoProgresses.GetByCourseAsync(request.CourseId, cancellationToken);
        var lectures = await _unitOfWork.Lectures.GetByCourseAsync(request.CourseId, cancellationToken);

        var totalWatchTime = sessions.Sum(s => s.DurationWatchedSeconds);
        var avgCourseCompletion = enrollments.Any()
            ? progresses.GroupBy(p => p.StudentId).Average(g =>
            {
                var studentProgresses = g.ToList();
                var totalPercent = studentProgresses.Sum(p => p.CompletionPercent);
                return lectures.Count > 0 ? totalPercent / lectures.Count : 0;
            })
            : 0;

        var lectureAnalytics = new List<LectureAnalyticsDto>();
        foreach (var lecture in lectures)
        {
            var lectureSessions = sessions.Where(s => s.LectureId == lecture.Id).ToList();
            var lectureProgresses = progresses.Where(p => p.LectureId == lecture.Id).ToList();
            lectureAnalytics.Add(new LectureAnalyticsDto
            {
                LectureId = lecture.Id,
                LectureTitle = lecture.Title,
                TotalWatchTimeSeconds = lectureSessions.Sum(s => s.DurationWatchedSeconds),
                UniqueWatchers = lectureSessions.Select(s => s.StudentId).Distinct().Count(),
                CompletionCount = lectureProgresses.Count(p => p.IsCompleted),
                DropOffCount = lectureSessions.Count(s => !s.IsCompleted),
                AverageCompletionPercent = lectureProgresses.Any() ? lectureProgresses.Average(p => p.CompletionPercent) : 0
            });
        }

        return new CourseAnalyticsDto
        {
            CourseId = course.Id,
            CourseTitle = course.Title,
            TotalEnrolledStudents = enrollments.Count,
            TotalWatchTimeSeconds = totalWatchTime,
            AverageCourseCompletionPercent = avgCourseCompletion,
            Lectures = lectureAnalytics
        };
    }
}
