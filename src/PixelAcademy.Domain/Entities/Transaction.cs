using System;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class Transaction : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public TransactionType Type { get; set; }
    public decimal? Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? ActivationCodeId { get; set; }
    public ActivationCode? ActivationCode { get; set; }
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
    public Guid? LectureId { get; set; }
    public Lecture? Lecture { get; set; }
}
