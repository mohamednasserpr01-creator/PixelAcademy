using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Commands.Enrollments;

public class UpdateEnrollmentStatusCommand : ICommand
{
    public Guid EnrollmentId { get; set; }
    public EnrollmentStatus Status { get; set; }
}
