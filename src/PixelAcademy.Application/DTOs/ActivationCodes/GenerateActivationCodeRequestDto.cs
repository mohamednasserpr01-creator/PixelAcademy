using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.ActivationCodes;

public class GenerateActivationCodeRequestDto
{
    public CodeType Type { get; set; }
    public decimal? Value { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LectureId { get; set; }
    public int? MaxRedemptions { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
