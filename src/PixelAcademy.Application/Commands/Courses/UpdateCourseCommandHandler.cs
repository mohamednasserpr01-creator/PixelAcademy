using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Courses;

public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, CourseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateCourseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<CourseDto> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.Id);

        if (!string.IsNullOrEmpty(request.Title)) course.Title = request.Title;
        if (request.Description != null) course.Description = request.Description;
        if (request.ShortDescription != null) course.ShortDescription = request.ShortDescription;
        if (request.Level.HasValue) course.Level = request.Level.Value;
        if (request.Status.HasValue) course.Status = request.Status.Value;
        if (request.Price.HasValue) course.Price = request.Price.Value;
        if (request.Category != null) course.Category = request.Category;
        if (request.Tags != null) course.Tags = request.Tags;

        course.UpdatedAt = _dateTimeProvider.UtcNow;
        await _unitOfWork.Courses.UpdateAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CourseDto>(course);
    }
}
