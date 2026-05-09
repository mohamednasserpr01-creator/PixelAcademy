namespace PixelAcademy.Application.DTOs.WatchSessions;

public class StartWatchSessionRequestDto
{
    public Guid LectureId { get; set; }
    public Guid CourseId { get; set; }
}
