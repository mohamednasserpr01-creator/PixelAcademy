namespace PixelAcademy.Application.DTOs.WatchSessions;

public class UpdateWatchProgressRequestDto
{
    public Guid LectureId { get; set; }
    public int CurrentPositionSeconds { get; set; }
    public int DurationWatchedSeconds { get; set; }
}
