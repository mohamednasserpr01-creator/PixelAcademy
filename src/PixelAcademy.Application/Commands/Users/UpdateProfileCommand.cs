using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Users;

namespace PixelAcademy.Application.Commands.Users;

public class UpdateProfileCommand : ICommand<UserProfileDto>
{
    public Guid UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
}
