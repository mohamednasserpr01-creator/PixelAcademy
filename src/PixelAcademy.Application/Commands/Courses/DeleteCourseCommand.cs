using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.Courses;

public class DeleteCourseCommand : ICommand
{
    public Guid Id { get; set; }
    public Guid RequestedById { get; set; }
}
