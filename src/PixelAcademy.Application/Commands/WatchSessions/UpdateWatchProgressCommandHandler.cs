using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Domain.Specifications;

namespace PixelAcademy.Application.Commands.WatchSessions;

public class UpdateWatchProgressCommandHandler : IRequestHandler<UpdateWatchProgressCommand, WatchSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateWatchProgressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WatchSessionDto> Handle(UpdateWatchProgressCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        var totalSeconds = lecture.DurationMinutes * 60;

        // Update or create VideoProgress
        var spec = new VideoProgressByStudentAndLectureSpec(request.StudentId, request.LectureId);
        var progress = await _unitOfWork.VideoProgresses.GetFirstAsync(spec, cancellationToken);

        if (progress == null)
        {
            progress = new VideoProgress
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                LectureId = request.LectureId,
                CourseId = lecture.CourseId,
                TotalSeconds = totalSeconds,
                CreatedAt = _dateTimeProvider.UtcNow
            };
            await _unitOfWork.VideoProgresses.AddAsync(progress, cancellationToken);
        }

        progress.WatchedSeconds = Math.Max(progress.WatchedSeconds, request.CurrentPositionSeconds);
        progress.WatchedSeconds = Math.Min(progress.WatchedSeconds, totalSeconds);
        progress.CompletionPercent = totalSeconds > 0 ? (int)((double)progress.WatchedSeconds / totalSeconds * 100) : 0;
        progress.IsCompleted = progress.CompletionPercent >= 85;
        progress.LastWatchedAt = _dateTimeProvider.UtcNow;
        progress.UpdatedAt = _dateTimeProvider.UtcNow;

        // Update latest watch session
        var sessions = await _unitOfWork.WatchSessions.GetByStudentAndLectureAsync(request.StudentId, request.LectureId, cancellationToken);
        var latestSession = sessions.OrderByDescending(s => s.StartedAt).FirstOrDefault();
        if (latestSession != null && !latestSession.EndedAt.HasValue)
        {
            latestSession.DurationWatchedSeconds = request.DurationWatchedSeconds;
            latestSession.LastPositionSeconds = request.CurrentPositionSeconds;
            latestSession.IsCompleted = progress.IsCompleted;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WatchSessionDto>(latestSession ?? new WatchSession { LectureId = request.LectureId });
    }
}
