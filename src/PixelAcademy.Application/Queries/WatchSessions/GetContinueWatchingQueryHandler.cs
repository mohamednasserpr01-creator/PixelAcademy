using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.WatchSessions;

public class GetContinueWatchingQueryHandler : IRequestHandler<GetContinueWatchingQuery, ContinueWatchingDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetContinueWatchingQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ContinueWatchingDto?> Handle(GetContinueWatchingQuery request, CancellationToken cancellationToken)
    {
        var latestSession = await _unitOfWork.WatchSessions.GetLatestByStudentAsync(request.StudentId, cancellationToken);
        if (latestSession == null) return null;

        var progress = await _unitOfWork.VideoProgresses.GetByStudentAndLectureAsync(
            request.StudentId, latestSession.LectureId, cancellationToken);

        return new ContinueWatchingDto
        {
            LectureId = latestSession.LectureId,
            LectureTitle = latestSession.Lecture.Title,
            CourseId = latestSession.CourseId,
            CourseTitle = latestSession.Lecture.Course.Title,
            LastPositionSeconds = progress?.WatchedSeconds ?? 0,
            CompletionPercent = progress?.CompletionPercent ?? 0,
            IsCompleted = progress?.IsCompleted ?? false
        };
    }
}
