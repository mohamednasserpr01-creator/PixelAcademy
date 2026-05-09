using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Transactions;

public class TransactionDto
{
    public Guid Id { get; set; }
    public TransactionType Type { get; set; }
    public decimal? Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid? ActivationCodeId { get; set; }
    public string? Code { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public Guid? LectureId { get; set; }
    public string? LectureTitle { get; set; }
    public DateTime CreatedAt { get; set; }
}
