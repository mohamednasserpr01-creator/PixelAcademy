using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Wallet;

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public WalletTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public Guid? LectureId { get; set; }
    public string? LectureTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}
