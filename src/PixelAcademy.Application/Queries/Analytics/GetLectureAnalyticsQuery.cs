using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Analytics;

namespace PixelAcademy.Application.Queries.Analytics;

public class GetLectureAnalyticsQuery : IQuery<LectureAnalyticsDto>
{
    public Guid LectureId { get; set; }
}
