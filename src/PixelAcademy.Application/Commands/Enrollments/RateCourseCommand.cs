using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.Enrollments;

public class RateCourseCommand : ICommand
{
    public Guid EnrollmentId { get; set; }
    public int Rating { get; set; }
    public string? Review { get; set; }
}
