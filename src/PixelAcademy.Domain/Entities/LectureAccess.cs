using System;
using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class LectureAccess : AuditableEntity
{
    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;
    public Guid LectureId { get; set; }
    public Lecture Lecture { get; set; } = null!;
    public Guid? ActivationCodeId { get; set; }
    public ActivationCode? ActivationCode { get; set; }
}
