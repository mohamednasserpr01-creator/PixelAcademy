using System;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class ActivationCode : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public CodeType Type { get; set; }
    public decimal? Value { get; set; }
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
    public Guid? LectureId { get; set; }
    public Lecture? Lecture { get; set; }
    public int MaxRedemptions { get; set; } = 1;
    public int CurrentRedemptions { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid GeneratedById { get; set; }
    public User GeneratedBy { get; set; } = null!;
    public Guid? LastRedeemedById { get; set; }
    public User? LastRedeemedBy { get; set; }
    public DateTime? LastRedeemedAt { get; set; }

    public bool IsFullyRedeemed => CurrentRedemptions >= MaxRedemptions;
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
}
