using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.ContentItems;

public class DeleteContentItemCommand : ICommand
{
    public Guid Id { get; set; }
}
