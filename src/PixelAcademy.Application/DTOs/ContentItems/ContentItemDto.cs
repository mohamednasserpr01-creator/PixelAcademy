using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.ContentItems;

public class ContentItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public ContentItemType Type { get; set; }
    public bool IsRequired { get; set; }
    public int? DurationSeconds { get; set; }
    public string? ExternalUrl { get; set; }
    public Guid LectureId { get; set; }
    public Guid? MediaAssetId { get; set; }
    public string? MediaAssetUrl { get; set; }
}
