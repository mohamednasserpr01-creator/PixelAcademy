using System;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Announcements;

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementTarget Target { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? ScheduledPublishAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAnnouncementRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public AnnouncementTarget Target { get; set; } = AnnouncementTarget.All;
    public Guid? CourseId { get; set; }
    public DateTime? ScheduledPublishAt { get; set; }
    public int Priority { get; set; }
}

public class UpdateAnnouncementRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime? ScheduledPublishAt { get; set; }
    public int Priority { get; set; }
}
