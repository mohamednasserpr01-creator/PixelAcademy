namespace PixelAcademy.Application.DTOs.WatchSessions;

public class FinishWatchSessionRequestDto
{
    public Guid LectureId { get; set; }
    public int FinalPositionSeconds { get; set; }
    public int TotalDurationWatchedSeconds { get; set; }
}
