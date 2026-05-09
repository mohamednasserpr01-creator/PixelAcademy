using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Domain.Specifications;

namespace PixelAcademy.Application.Queries.Courses;

public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CourseDetailDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = new CourseWithInstructorAndLecturesSpec(request.Id);
        var course = await _unitOfWork.Courses.GetFirstAsync(spec, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.Id);

        var dto = _mapper.Map<CourseDetailDto>(course);

        if (request.CurrentUserId.HasValue)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByStudentAndCourseAsync(
                request.CurrentUserId.Value, request.Id, cancellationToken);
            dto.IsEnrolled = enrollment != null;

            if (enrollment != null)
                dto.UserProgressPercent = enrollment.ProgressPercent;
        }

        return dto;
    }
}
