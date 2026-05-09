using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Domain.Specifications;

namespace PixelAcademy.Application.Commands.WatchSessions;

public class FinishWatchSessionCommandHandler : IRequestHandler<FinishWatchSessionCommand, WatchSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public FinishWatchSessionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WatchSessionDto> Handle(FinishWatchSessionCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        var totalSeconds = lecture.DurationMinutes * 60;

        // Find latest open session
        var sessions = await _unitOfWork.WatchSessions.GetByStudentAndLectureAsync(request.StudentId, request.LectureId, cancellationToken);
        var session = sessions.OrderByDescending(s => s.StartedAt).FirstOrDefault(s => !s.EndedAt.HasValue);
        if (session == null)
            throw new BadRequestException("No active watch session found for this lecture.");

        session.EndedAt = _dateTimeProvider.UtcNow;
        session.DurationWatchedSeconds = request.TotalDurationWatchedSeconds;
        session.LastPositionSeconds = request.FinalPositionSeconds;
        session.UpdatedAt = _dateTimeProvider.UtcNow;

        // Update VideoProgress final state
        var spec = new VideoProgressByStudentAndLectureSpec(request.StudentId, request.LectureId);
        var progress = await _unitOfWork.VideoProgresses.GetFirstAsync(spec, cancellationToken);

        if (progress == null)
        {
            progress = new Domain.Entities.VideoProgress
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

        progress.WatchedSeconds = Math.Max(progress.WatchedSeconds, request.FinalPositionSeconds);
        progress.WatchedSeconds = Math.Min(progress.WatchedSeconds, totalSeconds);
        progress.CompletionPercent = totalSeconds > 0 ? (int)((double)progress.WatchedSeconds / totalSeconds * 100) : 0;
        progress.IsCompleted = progress.CompletionPercent >= 85;
        progress.LastWatchedAt = _dateTimeProvider.UtcNow;
        progress.UpdatedAt = _dateTimeProvider.UtcNow;

        session.IsCompleted = progress.IsCompleted;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WatchSessionDto>(session);
    }
}
