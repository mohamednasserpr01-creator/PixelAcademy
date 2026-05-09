using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Auth;

namespace PixelAcademy.Application.Commands.Auth;

public class RegisterCommand : ICommand<AuthResponseDto>
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
