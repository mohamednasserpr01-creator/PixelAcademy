using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Auth;

namespace PixelAcademy.Application.Queries.Auth;

public class GetCurrentUserQuery : IQuery<UserDto>
{
    public Guid UserId { get; set; }
}
