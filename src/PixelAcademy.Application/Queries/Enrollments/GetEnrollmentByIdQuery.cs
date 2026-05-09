using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Enrollments;

namespace PixelAcademy.Application.Queries.Enrollments;

public class GetEnrollmentByIdQuery : IQuery<EnrollmentDto>
{
    public Guid Id { get; set; }
}
