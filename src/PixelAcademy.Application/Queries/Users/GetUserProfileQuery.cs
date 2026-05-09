using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Users;

namespace PixelAcademy.Application.Queries.Users;

public class GetUserProfileQuery : IQuery<UserProfileDto>
{
    public Guid UserId { get; set; }
}
