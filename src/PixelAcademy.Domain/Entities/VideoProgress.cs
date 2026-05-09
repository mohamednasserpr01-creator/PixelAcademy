using System;
using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class VideoProgress : AuditableEntity
{
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;
    public Guid LectureId { get; set; }
    public Lecture Lecture { get; set; } = null!;
    public Guid CourseId { get; set; }
    public int WatchedSeconds { get; set; }
    public int TotalSeconds { get; set; }
    public int CompletionPercent { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? LastWatchedAt { get; set; }
}
