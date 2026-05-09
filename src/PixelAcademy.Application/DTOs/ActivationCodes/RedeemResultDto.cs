using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.ActivationCodes;

public class RedeemResultDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public CodeType Type { get; set; }
    public decimal? NewWalletBalance { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LectureId { get; set; }
    public int RemainingRedemptions { get; set; }
}
