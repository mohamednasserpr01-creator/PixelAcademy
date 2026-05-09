namespace PixelAcademy.Application.DTOs.Progress;

public class UpdateProgressRequestDto
{
    public Guid LectureId { get; set; }
    public int WatchedSeconds { get; set; }
    public bool IsCompleted { get; set; }
}
