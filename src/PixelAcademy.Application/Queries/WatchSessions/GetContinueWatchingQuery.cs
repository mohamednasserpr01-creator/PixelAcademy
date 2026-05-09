using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.WatchSessions;

namespace PixelAcademy.Application.Queries.WatchSessions;

public class GetContinueWatchingQuery : IQuery<ContinueWatchingDto?>
{
    public Guid StudentId { get; set; }
}
