namespace PixelAcademy.Application.DTOs.Progress;

public class CourseProgressDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int TotalLectures { get; set; }
    public int CompletedLectures { get; set; }
    public int OverallCompletionPercent { get; set; }
    public List<LectureProgressSummaryDto> Lectures { get; set; } = new();
}
