using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.ActivationCodes;

public class ActivationCodeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public CodeType Type { get; set; }
    public decimal? Value { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public Guid? LectureId { get; set; }
    public string? LectureTitle { get; set; }
    public int MaxRedemptions { get; set; }
    public int CurrentRedemptions { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsFullyRedeemed { get; set; }
    public bool IsExpired { get; set; }
    public Guid GeneratedById { get; set; }
    public string GeneratedByName { get; set; } = string.Empty;
    public Guid? LastRedeemedById { get; set; }
    public DateTime? LastRedeemedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
