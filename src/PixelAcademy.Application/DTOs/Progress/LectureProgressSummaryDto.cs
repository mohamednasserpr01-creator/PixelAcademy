namespace PixelAcademy.Application.DTOs.Progress;

public class LectureProgressSummaryDto
{
    public Guid LectureId { get; set; }
    public string LectureTitle { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int CompletionPercent { get; set; }
    public bool IsCompleted { get; set; }
    public int LastPositionSeconds { get; set; }
}
