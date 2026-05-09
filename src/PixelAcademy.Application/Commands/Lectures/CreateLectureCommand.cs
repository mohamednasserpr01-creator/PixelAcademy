using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Lectures;

namespace PixelAcademy.Application.Commands.Lectures;

public class CreateLectureCommand : ICommand<LectureDto>
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsPreview { get; set; }
    public string? VideoUrl { get; set; }
}
