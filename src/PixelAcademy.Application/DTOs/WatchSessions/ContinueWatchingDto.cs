namespace PixelAcademy.Application.DTOs.WatchSessions;

public class ContinueWatchingDto
{
    public Guid LectureId { get; set; }
    public string LectureTitle { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int LastPositionSeconds { get; set; }
    public int CompletionPercent { get; set; }
    public bool IsCompleted { get; set; }
}
