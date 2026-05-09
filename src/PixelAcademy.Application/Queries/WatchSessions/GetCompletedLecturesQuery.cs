using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Progress;

namespace PixelAcademy.Application.Queries.WatchSessions;

public class GetCompletedLecturesQuery : IQuery<List<LectureProgressSummaryDto>>
{
    public Guid StudentId { get; set; }
    public Guid? CourseId { get; set; }
}
