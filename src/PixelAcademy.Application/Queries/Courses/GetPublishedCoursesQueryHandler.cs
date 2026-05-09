using AutoMapper;
using MediatR;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Domain.Specifications;

namespace PixelAcademy.Application.Queries.Courses;

public class GetPublishedCoursesQueryHandler : IRequestHandler<GetPublishedCoursesQuery, PagedResponse<CourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPublishedCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<CourseDto>> Handle(GetPublishedCoursesQuery request, CancellationToken cancellationToken)
    {
        var spec = new CoursePublishedSpec();
        var courses = await _unitOfWork.Courses.GetAsync(spec, cancellationToken);
        var query = courses.AsQueryable();

        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(c => c.Title.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(request.Category))
            query = query.Where(c => c.Category == request.Category);

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
