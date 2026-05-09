using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Courses;

namespace PixelAcademy.Application.Queries.Courses;

public class GetPublishedCoursesQuery : IQuery<PagedResponse<CourseDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Category { get; set; }
    public string? Search { get; set; }
}
