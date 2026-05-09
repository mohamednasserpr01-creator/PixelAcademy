using System;

namespace PixelAcademy.Application.DTOs.Assignments;

public class AssignmentSubmissionDto
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string? TextAnswer { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    public string? GradedByName { get; set; }
    public DateTime? GradedAt { get; set; }
    public bool IsLate { get; set; }
    public int MaxPoints { get; set; }
}

public class SubmitAssignmentRequestDto
{
    public Guid AssignmentId { get; set; }
    public string? TextAnswer { get; set; }
    public string? FileUrl { get; set; }
}

public class GradeAssignmentRequestDto
{
    public Guid SubmissionId { get; set; }
    public int Score { get; set; }
    public string? Feedback { get; set; }
}
