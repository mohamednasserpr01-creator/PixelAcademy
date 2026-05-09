using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.ContentItems;

public class CreateContentItemRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public ContentItemType Type { get; set; }
    public bool IsRequired { get; set; } = true;
    public int? DurationSeconds { get; set; }
    public string? ExternalUrl { get; set; }
    public Guid? MediaAssetId { get; set; }
}
