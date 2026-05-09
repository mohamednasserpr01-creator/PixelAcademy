using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Enrollments;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Enrollments;

public class CreateEnrollmentCommandHandler : IRequestHandler<CreateEnrollmentCommand, EnrollmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateEnrollmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<EnrollmentDto> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.CourseId);

        if (course.Status != CourseStatus.Published)
            throw new BadRequestException("Course is not published.");

        if (await _unitOfWork.Enrollments.IsEnrolledAsync(request.StudentId, request.CourseId, cancellationToken))
            throw new ConflictException("Already enrolled in this course.");

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            Status = EnrollmentStatus.Active,
            ProgressPercent = 0,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EnrollmentDto>(enrollment);
    }
}
