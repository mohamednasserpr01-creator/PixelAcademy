using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.ContentItems;

namespace PixelAcademy.Application.Queries.ContentItems;

public class GetContentItemByIdQuery : IQuery<ContentItemDto>
{
    public Guid Id { get; set; }
}
