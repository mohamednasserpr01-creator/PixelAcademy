using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.Users;

public class ChangePasswordCommand : ICommand
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
