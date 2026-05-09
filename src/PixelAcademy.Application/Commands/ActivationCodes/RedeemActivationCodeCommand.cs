using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.ActivationCodes;

namespace PixelAcademy.Application.Commands.ActivationCodes;

public class RedeemActivationCodeCommand : ICommand<RedeemResultDto>
{
    public string Code { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
}
