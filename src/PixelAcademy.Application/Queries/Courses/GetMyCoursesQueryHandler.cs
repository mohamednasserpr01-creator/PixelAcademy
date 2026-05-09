using AutoMapper;
using MediatR;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Courses;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Courses;

public class GetMyCoursesQueryHandler : IRequestHandler<GetMyCoursesQuery, PagedResponse<CourseDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetMyCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResponse<CourseDto>> Handle(GetMyCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.Courses.GetByInstructorAsync(request.InstructorId, cancellationToken);
        var query = courses.AsQueryable();

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
