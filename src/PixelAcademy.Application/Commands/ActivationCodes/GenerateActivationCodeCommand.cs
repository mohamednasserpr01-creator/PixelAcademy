using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Commands.ActivationCodes;

public class GenerateActivationCodeCommand : ICommand<ActivationCodeDto>
{
    public CodeType Type { get; set; }
    public decimal? Value { get; set; }
    public Guid? CourseId { get; set; }
    public Guid? LectureId { get; set; }
    public int? MaxRedemptions { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid GeneratedById { get; set; }
}
