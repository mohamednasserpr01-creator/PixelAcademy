using PixelAcademy.Application.Abstractions.Mediator;

namespace PixelAcademy.Application.Commands.Lectures;

public class DeleteLectureCommand : ICommand
{
    public Guid Id { get; set; }
}
