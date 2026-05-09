using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class Assignment : SoftDeleteEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
    public Guid? LectureId { get; set; }
    public Lecture? Lecture { get; set; }
    public DateTime? DueDate { get; set; }
    public int MaxPoints { get; set; } = 100;
    public bool AllowLateSubmission { get; set; }
    public bool IsPublished { get; set; }
    public Guid CreatedById { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
}
