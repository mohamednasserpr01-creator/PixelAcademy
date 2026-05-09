using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.WatchSessions;

namespace PixelAcademy.Application.Commands.WatchSessions;

public class StartWatchSessionCommand : ICommand<WatchSessionDto>
{
    public Guid StudentId { get; set; }
    public Guid LectureId { get; set; }
    public Guid CourseId { get; set; }
}
