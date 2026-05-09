using AutoMapper;
using MediatR;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Auth;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Users;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResponse<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var query = users.AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(u => u.Email.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                || u.Username.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                || u.FirstName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(request.Role) && Enum.TryParse<UserRole>(request.Role, true, out var role))
            query = query.Where(u => u.Role == role);

        query = query.Where(u => !u.IsDeleted);

        var totalCount = query.Count();
        var items = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResponse<UserDto>
        {
            Items = _mapper.Map<IReadOnlyList<UserDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
