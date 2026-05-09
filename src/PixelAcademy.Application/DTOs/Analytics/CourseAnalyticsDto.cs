namespace PixelAcademy.Application.DTOs.Analytics;

public class CourseAnalyticsDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int TotalEnrolledStudents { get; set; }
    public int TotalWatchTimeSeconds { get; set; }
    public double AverageCourseCompletionPercent { get; set; }
    public List<LectureAnalyticsDto> Lectures { get; set; } = new();
}
