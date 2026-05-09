using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class ExamAttempt : AuditableEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;
    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int? Score { get; set; }
    public int? TotalPoints { get; set; }
    public bool? IsPassed { get; set; }
    public int? DurationSeconds { get; set; }
    public ExamAttemptStatus Status { get; set; } = ExamAttemptStatus.InProgress;

    public ICollection<ExamAnswer> Answers { get; set; } = new List<ExamAnswer>();
}
