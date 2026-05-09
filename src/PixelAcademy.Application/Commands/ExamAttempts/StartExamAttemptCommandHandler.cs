using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.ExamAttempts;

public class StartExamAttemptCommandHandler : IRequestHandler<StartExamAttemptCommand, ExamAttemptDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public StartExamAttemptCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ExamAttemptDto> Handle(StartExamAttemptCommand request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(request.ExamId, cancellationToken);
        if (exam == null) throw new NotFoundException("Exam", request.ExamId);
        if (!exam.IsPublished) throw new ForbiddenException("This exam is not published.");
        if (exam.StartDate.HasValue && exam.StartDate.Value > _dateTimeProvider.UtcNow)
            throw new BadRequestException("This exam has not started yet.");
        if (exam.EndDate.HasValue && exam.EndDate.Value < _dateTimeProvider.UtcNow)
            throw new BadRequestException("This exam has ended.");

        // Validate enrollment
        if (exam.CourseId.HasValue)
        {
            var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.StudentId, exam.CourseId.Value, cancellationToken);
            if (!isEnrolled)
                throw new ForbiddenException("You must be enrolled in the course to take this exam.");
        }

        // Check attempt limit
        var attemptCount = await _unitOfWork.ExamAttempts.CountAttemptsAsync(request.StudentId, request.ExamId, cancellationToken);
        if (attemptCount >= exam.AttemptLimit)
            throw new BadRequestException($"You have reached the maximum attempt limit of {exam.AttemptLimit}.");

        // Prevent multiple simultaneous attempts
        var inProgress = await _unitOfWork.ExamAttempts.GetInProgressAsync(request.StudentId, request.ExamId, cancellationToken);
        if (inProgress != null)
            throw new BadRequestException("You already have an in-progress attempt. Finish or abandon it first.");

        var attempt = new ExamAttempt
        {
            Id = Guid.NewGuid(),
            ExamId = request.ExamId,
            StudentId = request.StudentId,
            StartedAt = _dateTimeProvider.UtcNow,
            Status = ExamAttemptStatus.InProgress,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.ExamAttempts.AddAsync(attempt, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ExamAttemptDto>(attempt);
    }
}
