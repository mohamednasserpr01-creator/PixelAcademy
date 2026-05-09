using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Media;

public class UploadMediaRequestDto
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public MediaType Type { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public int? DurationSeconds { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LectureId { get; set; }
}
