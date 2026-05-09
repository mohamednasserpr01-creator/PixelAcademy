using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.WatchSessions;

namespace PixelAcademy.Application.Commands.WatchSessions;

public class GenerateSignedVideoUrlCommand : ICommand<SignedVideoUrlDto>
{
    public Guid StudentId { get; set; }
    public Guid LectureId { get; set; }
}
