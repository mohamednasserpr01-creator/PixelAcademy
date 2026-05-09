using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.WatchSessions;

namespace PixelAcademy.Application.Commands.WatchSessions;

public class UpdateWatchProgressCommand : ICommand<WatchSessionDto>
{
    public Guid StudentId { get; set; }
    public Guid LectureId { get; set; }
    public int CurrentPositionSeconds { get; set; }
    public int DurationWatchedSeconds { get; set; }
}
