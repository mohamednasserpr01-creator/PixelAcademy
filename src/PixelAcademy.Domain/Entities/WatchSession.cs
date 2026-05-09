using System;
using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class WatchSession : AuditableEntity
{
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;
    public Guid LectureId { get; set; }
    public Lecture Lecture { get; set; } = null!;
    public Guid CourseId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public int DurationWatchedSeconds { get; set; }
    public int? LastPositionSeconds { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsCompleted { get; set; }
}
