using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.WatchSessions;

namespace PixelAcademy.Application.Commands.WatchSessions;

public class FinishWatchSessionCommand : ICommand<WatchSessionDto>
{
    public Guid StudentId { get; set; }
    public Guid LectureId { get; set; }
    public int FinalPositionSeconds { get; set; }
    public int TotalDurationWatchedSeconds { get; set; }
}
