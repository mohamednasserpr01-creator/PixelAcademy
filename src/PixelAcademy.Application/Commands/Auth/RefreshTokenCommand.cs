using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Auth;

namespace PixelAcademy.Application.Commands.Auth;

public class RefreshTokenCommand : ICommand<AuthResponseDto>
{
    public string RefreshToken { get; set; } = string.Empty;
}
