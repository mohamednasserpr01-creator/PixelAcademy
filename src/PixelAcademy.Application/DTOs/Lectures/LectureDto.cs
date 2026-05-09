namespace PixelAcademy.Application.DTOs.Lectures;

public class LectureDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsPreview { get; set; }
    public string? VideoUrl { get; set; }
    public Guid CourseId { get; set; }
    public bool IsWatched { get; set; }
    public int? WatchedSeconds { get; set; }
}
