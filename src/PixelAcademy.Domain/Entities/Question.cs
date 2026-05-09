using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class Question : SoftDeleteEntity
{
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public QuestionType Type { get; set; } = QuestionType.MultipleChoice;
    public int OrderIndex { get; set; }
    public int Points { get; set; } = 1;

    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    public ICollection<ExamAnswer> Answers { get; set; } = new List<ExamAnswer>();
}
