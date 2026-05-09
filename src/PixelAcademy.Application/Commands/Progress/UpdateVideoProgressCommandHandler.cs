using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Progress;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Domain.Specifications;

namespace PixelAcademy.Application.Commands.Progress;

public class UpdateVideoProgressCommandHandler : IRequestHandler<UpdateVideoProgressCommand, VideoProgressDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateVideoProgressCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<VideoProgressDto> Handle(UpdateVideoProgressCommand request, CancellationToken cancellationToken)
    {
        var spec = new VideoProgressByStudentAndLectureSpec(request.StudentId, request.LectureId);
        var progress = await _unitOfWork.VideoProgresses.GetFirstAsync(spec, cancellationToken);

        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        if (progress == null)
        {
            progress = new VideoProgress
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                LectureId = request.LectureId,
                CourseId = lecture.CourseId,
                TotalSeconds = lecture.DurationMinutes * 60,
                CreatedAt = _dateTimeProvider.UtcNow
            };
            await _unitOfWork.VideoProgresses.AddAsync(progress, cancellationToken);
        }

        progress.WatchedSeconds = Math.Min(request.WatchedSeconds, progress.TotalSeconds);
        progress.IsCompleted = request.IsCompleted || progress.WatchedSeconds >= progress.TotalSeconds * 0.9;
        progress.LastWatchedAt = _dateTimeProvider.UtcNow;
        progress.UpdatedAt = _dateTimeProvider.UtcNow;

        await _unitOfWork.VideoProgresses.UpdateAsync(progress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<VideoProgressDto>(progress);
    }
}
