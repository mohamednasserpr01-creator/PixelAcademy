using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Exams;

public class QuestionDto
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public QuestionType Type { get; set; }
    public int OrderIndex { get; set; }
    public int Points { get; set; }
    public List<QuestionOptionDto> Options { get; set; } = new();
}

public class QuestionOptionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}

public class QuestionOptionDetailDto : QuestionOptionDto
{
    public bool IsCorrect { get; set; }
}

public class QuestionDetailDto : QuestionDto
{
    public new List<QuestionOptionDetailDto> Options { get; set; } = new();
}

public class CreateQuestionRequestDto
{
    public Guid ExamId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public QuestionType Type { get; set; } = QuestionType.MultipleChoice;
    public int OrderIndex { get; set; }
    public int Points { get; set; } = 1;
    public List<CreateQuestionOptionRequestDto> Options { get; set; } = new();
}

public class CreateQuestionOptionRequestDto
{
    public string Text { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public bool IsCorrect { get; set; }
}

public class UpdateQuestionRequestDto
{
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int OrderIndex { get; set; }
    public int Points { get; set; } = 1;
    public List<CreateQuestionOptionRequestDto> Options { get; set; } = new();
}
