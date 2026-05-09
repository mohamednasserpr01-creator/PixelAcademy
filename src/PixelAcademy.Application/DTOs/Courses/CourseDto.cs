using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Courses;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; }
    public decimal? Price { get; set; }
    public int DurationMinutes { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int LectureCount { get; set; }
    public int EnrollmentCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
