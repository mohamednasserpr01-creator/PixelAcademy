using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Courses;

namespace PixelAcademy.Application.Queries.Courses;

public class GetCourseByIdQuery : IQuery<CourseDetailDto>
{
    public Guid Id { get; set; }
    public Guid? CurrentUserId { get; set; }
}
