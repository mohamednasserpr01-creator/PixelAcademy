using System;
using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class ExamAnswer : AuditableEntity
{
    public Guid ExamAttemptId { get; set; }
    public ExamAttempt ExamAttempt { get; set; } = null!;
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public string? SelectedOptionIds { get; set; }
    public string? TextAnswer { get; set; }
    public bool? IsCorrect { get; set; }
    public int? PointsEarned { get; set; }
}
