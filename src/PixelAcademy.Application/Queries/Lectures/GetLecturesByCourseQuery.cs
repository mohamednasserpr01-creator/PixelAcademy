using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Lectures;

namespace PixelAcademy.Application.Queries.Lectures;

public class GetLecturesByCourseQuery : IQuery<List<LectureDto>>
{
    public Guid CourseId { get; set; }
    public Guid? StudentId { get; set; }
}
