using AutoMapper;
using MediatR;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Domain.Specifications;

namespace PixelAcademy.Application.Queries.Courses;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, PagedResponse<CourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAllCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
    {
        var spec = new CoursePublishedSpec();
        var all = await _unitOfWork.Courses.GetAsync(spec, cancellationToken);

        var query = all.AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(c => c.Title.Contains(request.Search, StringComparison.OrdinalIgnoreCase)
                || (c.Description != null && c.Description.Contains(request.Search, StringComparison.OrdinalIgnoreCase)));

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(c => c.Category == request.Category);

        if (!string.IsNullOrEmpty(request.Level) && Enum.TryParse<CourseLevel>(request.Level, true, out var level))
            query = query.Where(c => c.Level == level);

        var totalCount = query.Count();
        var items = query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResponse<CourseDto>
        {
            Items = _mapper.Map<IReadOnlyList<CourseDto>>(items),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
