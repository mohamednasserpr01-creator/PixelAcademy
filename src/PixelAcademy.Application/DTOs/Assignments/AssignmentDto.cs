using System;
using System.Collections.Generic;

namespace PixelAcademy.Application.DTOs.Assignments;

public class AssignmentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public Guid? LectureId { get; set; }
    public string? LectureTitle { get; set; }
    public DateTime? DueDate { get; set; }
    public int MaxPoints { get; set; }
    public bool AllowLateSubmission { get; set; }
    public bool IsPublished { get; set; }
    public int SubmissionCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAssignmentRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LectureId { get; set; }
    public DateTime? DueDate { get; set; }
    public int MaxPoints { get; set; } = 100;
    public bool AllowLateSubmission { get; set; }
}

public class UpdateAssignmentRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public DateTime? DueDate { get; set; }
    public int MaxPoints { get; set; } = 100;
    public bool AllowLateSubmission { get; set; }
}
