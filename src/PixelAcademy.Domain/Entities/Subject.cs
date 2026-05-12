using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}