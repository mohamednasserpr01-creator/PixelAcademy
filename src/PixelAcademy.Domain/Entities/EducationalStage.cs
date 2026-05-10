using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class EducationalStage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<EducationStream> EducationStreams { get; set; } = new List<EducationStream>();
}
