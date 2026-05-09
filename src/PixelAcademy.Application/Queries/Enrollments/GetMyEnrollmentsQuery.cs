using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.Abstractions.Pagination;
using PixelAcademy.Application.DTOs.Enrollments;

namespace PixelAcademy.Application.Queries.Enrollments;

public class GetMyEnrollmentsQuery : IQuery<PagedResponse<EnrollmentDto>>
{
    public Guid StudentId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
