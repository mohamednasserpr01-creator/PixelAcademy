using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.WatchSessions;

public class StartWatchSessionCommandHandler : IRequestHandler<StartWatchSessionCommand, WatchSessionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StartWatchSessionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WatchSessionDto> Handle(StartWatchSessionCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.CourseId);

        // Validate access: enrolled in course OR has lecture access
        var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.StudentId, request.CourseId, cancellationToken);
        var hasLectureAccess = await _unitOfWork.LectureAccesses.HasAccessAsync(request.StudentId, request.LectureId, cancellationToken);

        if (!isEnrolled && !hasLectureAccess && lecture.IsPreview == false)
            throw new ForbiddenException("You do not have access to this lecture.");

        var session = new WatchSession
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            LectureId = request.LectureId,
            CourseId = request.CourseId,
            StartedAt = _dateTimeProvider.UtcNow,
            DurationWatchedSeconds = 0,
            IsCompleted = false,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.WatchSessions.AddAsync(session, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<WatchSessionDto>(session);
    }
}
