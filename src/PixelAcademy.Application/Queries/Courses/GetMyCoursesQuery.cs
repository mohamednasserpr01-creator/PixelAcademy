using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Courses;

namespace PixelAcademy.Application.Queries.Courses;

public class GetMyCoursesQuery : IQuery<PagedResponse<CourseDto>>
{
    public Guid InstructorId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
