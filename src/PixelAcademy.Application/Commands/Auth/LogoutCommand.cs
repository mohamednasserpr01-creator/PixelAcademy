using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.Auth;

public class LogoutCommand : ICommand
{
    public Guid UserId { get; set; }
    public string? Token { get; set; }
}
