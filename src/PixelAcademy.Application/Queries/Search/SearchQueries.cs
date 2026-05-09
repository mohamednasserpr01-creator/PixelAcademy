using MediatR;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Application.DTOs.Search;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Queries.Search;

public record SearchQuery(string Query, int Page = 1, int PageSize = 20) : IRequest<SearchResultDto>;
public record SearchCoursesQuery(string? Query, string? Category, CourseLevel? Level, int Page = 1, int PageSize = 20) : IRequest<PagedResult<SearchResultDto>>;
