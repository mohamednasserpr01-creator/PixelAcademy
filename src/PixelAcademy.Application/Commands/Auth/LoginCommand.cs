using MediatR;
using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Auth;

namespace PixelAcademy.Application.Commands.Auth;

public class LoginCommand : ICommand<AuthResponseDto>
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
