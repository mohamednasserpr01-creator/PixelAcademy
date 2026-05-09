using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Exams;

public class ExamDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ExamType Type { get; set; }
    public Guid? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public Guid? LectureId { get; set; }
    public string? LectureTitle { get; set; }
    public int? DurationMinutes { get; set; }
    public int AttemptLimit { get; set; }
    public int PassScorePercent { get; set; }
    public bool IsPublished { get; set; }
    public bool IsRandomized { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool ShowCorrectAnswers { get; set; }
    public int QuestionCount { get; set; }
    public int TotalPoints { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ExamDetailDto : ExamDto
{
    public List<QuestionDto> Questions { get; set; } = new();
}

public class CreateExamRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ExamType Type { get; set; } = ExamType.CourseExam;
    public Guid? CourseId { get; set; }
    public Guid? LectureId { get; set; }
    public int? DurationMinutes { get; set; }
    public int AttemptLimit { get; set; } = 1;
    public int PassScorePercent { get; set; } = 50;
    public bool IsRandomized { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool ShowCorrectAnswers { get; set; }
}

public class UpdateExamRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? DurationMinutes { get; set; }
    public int AttemptLimit { get; set; } = 1;
    public int PassScorePercent { get; set; } = 50;
    public bool IsRandomized { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool ShowCorrectAnswers { get; set; }
}
