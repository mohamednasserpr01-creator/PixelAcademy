namespace PixelAcademy.Application.DTOs.WatchSessions;

public class WatchSessionDto
{
    public Guid Id { get; set; }
    public Guid LectureId { get; set; }
    public string LectureTitle { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int DurationWatchedSeconds { get; set; }
    public int? LastPositionSeconds { get; set; }
    public bool IsCompleted { get; set; }
}
