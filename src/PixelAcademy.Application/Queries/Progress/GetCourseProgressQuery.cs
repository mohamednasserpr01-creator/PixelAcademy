using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Progress;

namespace PixelAcademy.Application.Queries.Progress;

public class GetCourseProgressQuery : IQuery<List<VideoProgressDto>>
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
}
