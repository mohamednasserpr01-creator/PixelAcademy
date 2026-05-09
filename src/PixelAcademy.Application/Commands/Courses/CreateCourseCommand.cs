using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Commands.Courses;

public class CreateCourseCommand : ICommand<CourseDto>
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;
    public decimal? Price { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public Guid InstructorId { get; set; }
}
