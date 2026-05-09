using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Media;

public class MediaAssetDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public int? DurationSeconds { get; set; }
    public DateTime CreatedAt { get; set; }
}
