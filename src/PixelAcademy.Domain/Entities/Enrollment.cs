using System;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class Enrollment : AuditableEntity
{
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;
    public Guid CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public DateTime? CompletedAt { get; set; }
    public decimal ProgressPercent { get; set; }
    public int? Rating { get; set; }
    public string? Review { get; set; }
    public Guid? ActivationCodeId { get; set; }
    public ActivationCode? ActivationCode { get; set; }
}
