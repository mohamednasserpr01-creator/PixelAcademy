using System;
using PixelAcademy.Domain.Common;

namespace PixelAcademy.Domain.Entities;

public class QuestionOption : SoftDeleteEntity
{
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public bool IsCorrect { get; set; }
}
