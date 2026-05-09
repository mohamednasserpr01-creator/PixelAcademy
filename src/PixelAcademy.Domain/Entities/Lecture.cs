using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class Lecture : SoftDeleteEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsPreview { get; set; }
    public bool IsDisabled { get; set; }
    public string? VideoUrl { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<VideoProgress> VideoProgresses { get; set; } = new List<VideoProgress>();
    public ICollection<MediaAsset> MediaAssets { get; set; } = new List<MediaAsset>();
    public ICollection<ContentItem> ContentItems { get; set; } = new List<ContentItem>();
}
