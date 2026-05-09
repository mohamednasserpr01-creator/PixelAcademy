namespace PixelAcademy.Application.DTOs.Lectures;

public class CreateLectureRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsPreview { get; set; }
    public string? VideoUrl { get; set; }
}
