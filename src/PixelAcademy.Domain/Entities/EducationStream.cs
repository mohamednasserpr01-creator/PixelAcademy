using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class EducationStream : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public Guid EducationalStageId { get; set; }
    public EducationalStage EducationalStage { get; set; } = null!;
}
