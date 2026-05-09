using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Media;

namespace PixelAcademy.Application.Queries.Media;

public class GetMediaByCourseQuery : IQuery<List<MediaAssetDto>>
{
    public Guid CourseId { get; set; }
}
