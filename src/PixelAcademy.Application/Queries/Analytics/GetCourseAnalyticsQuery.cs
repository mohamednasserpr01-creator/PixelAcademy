using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Analytics;

namespace PixelAcademy.Application.Queries.Analytics;

public class GetCourseAnalyticsQuery : IQuery<CourseAnalyticsDto>
{
    public Guid CourseId { get; set; }
}
