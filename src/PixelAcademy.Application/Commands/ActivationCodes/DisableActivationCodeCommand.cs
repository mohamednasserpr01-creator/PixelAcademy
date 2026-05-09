using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.ActivationCodes;

public class DisableActivationCodeCommand : ICommand
{
    public Guid Id { get; set; }
    public Guid RequestedById { get; set; }
}
