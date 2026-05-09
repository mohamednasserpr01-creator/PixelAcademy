using System;
using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class AssignmentSubmission : AuditableEntity
{
    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = null!;
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;
    public string? TextAnswer { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int? Score { get; set; }
    public string? Feedback { get; set; }
    public string? GradedByName { get; set; }
    public DateTime? GradedAt { get; set; }
    public bool IsLate { get; set; }
}
