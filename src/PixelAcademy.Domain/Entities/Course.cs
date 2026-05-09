using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class Course : SoftDeleteEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TrailerUrl { get; set; }
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public bool IsDisabled { get; set; }
    public decimal? Price { get; set; }
    public int DurationMinutes { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public Guid InstructorId { get; set; }
    public User Instructor { get; set; } = null!;

    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<MediaAsset> MediaAssets { get; set; } = new List<MediaAsset>();
}
