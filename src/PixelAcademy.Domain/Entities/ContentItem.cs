using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class ContentItem : SoftDeleteEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public ContentItemType Type { get; set; }
    public bool IsRequired { get; set; } = true;
    public int? DurationSeconds { get; set; }
    public string? ExternalUrl { get; set; }
    public Guid LectureId { get; set; }
    public Lecture Lecture { get; set; } = null!;
    public Guid? MediaAssetId { get; set; }
    public MediaAsset? MediaAsset { get; set; }
}
