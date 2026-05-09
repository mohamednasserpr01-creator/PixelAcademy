using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Courses;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, CourseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateCourseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<CourseDto> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var instructor = await _unitOfWork.Users.GetByIdAsync(request.InstructorId, cancellationToken);
        if (instructor == null) throw new NotFoundException("User", request.InstructorId);

        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            Level = request.Level,
            Price = request.Price,
            Category = request.Category,
            Tags = request.Tags,
            InstructorId = request.InstructorId,
            Status = Domain.Enums.CourseStatus.Draft,
            CreatedAt = _dateTimeProvider.UtcNow,
            CreatedBy = instructor.Email
        };

        await _unitOfWork.Courses.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CourseDto>(course);
    }
}
