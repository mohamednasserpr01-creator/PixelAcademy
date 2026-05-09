using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Exams;

public class CreateExamCommandHandler : IRequestHandler<CreateExamCommand, ExamDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateExamCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ExamDto> Handle(CreateExamCommand request, CancellationToken cancellationToken)
    {
        if (request.CourseId.HasValue)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId.Value, cancellationToken);
            if (course == null) throw new NotFoundException("Course", request.CourseId.Value);
            if (course.InstructorId != request.CreatedById)
                throw new ForbiddenException("You can only create exams for your own courses.");
        }

        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            CourseId = request.CourseId,
            LectureId = request.LectureId,
            DurationMinutes = request.DurationMinutes,
            AttemptLimit = request.AttemptLimit,
            PassScorePercent = request.PassScorePercent,
            IsRandomized = request.IsRandomized,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ShowCorrectAnswers = request.ShowCorrectAnswers,
            CreatedById = request.CreatedById,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.Exams.AddAsync(exam, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ExamDto>(exam);
    }
}
