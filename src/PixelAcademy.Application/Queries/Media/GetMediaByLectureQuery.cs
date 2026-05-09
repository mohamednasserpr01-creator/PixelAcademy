using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.Media;

namespace PixelAcademy.Application.Queries.Media;

public class GetMediaByLectureQuery : IQuery<List<MediaAssetDto>>
{
    public Guid LectureId { get; set; }
}
