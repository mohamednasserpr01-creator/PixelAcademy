using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Enrollments;

namespace PixelAcademy.Application.Commands.Enrollments;

public class CreateEnrollmentCommand : ICommand<EnrollmentDto>
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
}
