using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Commands.Courses;

public class UpdateCourseCommand : ICommand<CourseDto>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public CourseLevel? Level { get; set; }
    public CourseStatus? Status { get; set; }
    public decimal? Price { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
}
