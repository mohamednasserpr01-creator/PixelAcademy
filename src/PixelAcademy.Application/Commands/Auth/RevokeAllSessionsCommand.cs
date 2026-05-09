using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.Auth;

public class RevokeAllSessionsCommand : ICommand
{
    public Guid UserId { get; set; }
}
