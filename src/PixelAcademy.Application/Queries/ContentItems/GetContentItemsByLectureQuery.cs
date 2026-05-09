using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.ContentItems;

namespace PixelAcademy.Application.Queries.ContentItems;

public class GetContentItemsByLectureQuery : IQuery<List<ContentItemDto>>
{
    public Guid LectureId { get; set; }
}
