namespace PixelAcademy.Application.DTOs.WatchSessions;

public class SignedVideoUrlDto
{
    public string SignedUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string LectureTitle { get; set; } = string.Empty;
}
