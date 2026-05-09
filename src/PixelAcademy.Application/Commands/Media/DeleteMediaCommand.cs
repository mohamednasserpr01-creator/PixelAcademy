using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.Media;

public class DeleteMediaCommand : ICommand
{
    public Guid Id { get; set; }
}
