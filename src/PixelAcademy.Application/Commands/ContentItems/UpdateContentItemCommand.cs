using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.ContentItems;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.Commands.ContentItems;

public class UpdateContentItemCommand : ICommand<ContentItemDto>
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? OrderIndex { get; set; }
    public ContentItemType? Type { get; set; }
    public bool? IsRequired { get; set; }
    public int? DurationSeconds { get; set; }
    public string? ExternalUrl { get; set; }
    public Guid? MediaAssetId { get; set; }
}
