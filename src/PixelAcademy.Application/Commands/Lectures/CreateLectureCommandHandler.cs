using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Lectures;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Lectures;

public class CreateLectureCommandHandler : IRequestHandler<CreateLectureCommand, LectureDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateLectureCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LectureDto> Handle(CreateLectureCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.CourseId);

        var instructor = await _unitOfWork.Users.GetByIdAsync(course.InstructorId, cancellationToken);

        var lecture = new Lecture
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            OrderIndex = request.OrderIndex,
            DurationMinutes = request.DurationMinutes,
            IsPreview = request.IsPreview,
            VideoUrl = request.VideoUrl,
            CourseId = request.CourseId,
            CreatedAt = _dateTimeProvider.UtcNow,
            CreatedBy = instructor?.PhoneNumber ?? "system"
        };

        await _unitOfWork.Lectures.AddAsync(lecture, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<LectureDto>(lecture);
    }
}
