namespace PixelAcademy.Application.DTOs.Analytics;

public class LectureAnalyticsDto
{
    public Guid LectureId { get; set; }
    public string LectureTitle { get; set; } = string.Empty;
    public int TotalWatchTimeSeconds { get; set; }
    public int UniqueWatchers { get; set; }
    public int CompletionCount { get; set; }
    public int DropOffCount { get; set; }
    public double AverageCompletionPercent { get; set; }
}
