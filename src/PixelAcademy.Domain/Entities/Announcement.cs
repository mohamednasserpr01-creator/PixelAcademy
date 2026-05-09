using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class Announcement : SoftDeleteEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementTarget Target { get; set; } = AnnouncementTarget.All;
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
    public Guid CreatedById { get; set; }
    public User Creator { get; set; } = null!;
    public bool IsPublished { get; set; }
    public DateTime? ScheduledPublishAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int Priority { get; set; }
}
