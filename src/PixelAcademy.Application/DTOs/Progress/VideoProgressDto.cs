namespace PixelAcademy.Application.DTOs.Progress;

public class VideoProgressDto
{
    public Guid Id { get; set; }
    public Guid LectureId { get; set; }
    public string LectureTitle { get; set; } = string.Empty;
    public int WatchedSeconds { get; set; }
    public int TotalSeconds { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? LastWatchedAt { get; set; }
}
