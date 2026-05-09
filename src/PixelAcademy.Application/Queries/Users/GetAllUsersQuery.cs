using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Auth;

namespace PixelAcademy.Application.Queries.Users;

public class GetAllUsersQuery : IQuery<PagedResponse<UserDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? Role { get; set; }
}
