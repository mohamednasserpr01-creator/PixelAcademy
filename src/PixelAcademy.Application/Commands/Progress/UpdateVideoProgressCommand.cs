using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Progress;

namespace PixelAcademy.Application.Commands.Progress;

public class UpdateVideoProgressCommand : ICommand<VideoProgressDto>
{
    public Guid StudentId { get; set; }
    public Guid LectureId { get; set; }
    public int WatchedSeconds { get; set; }
    public bool IsCompleted { get; set; }
}
