using System;
using System.Collections.Generic;

namespace PixelAcademy.Application.DTOs.Exams;

public class ExamAnalyticsDto
{
    public Guid ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public int TotalAttempts { get; set; }
    public int CompletedAttempts { get; set; }
    public double AverageScore { get; set; }
    public double AveragePercentage { get; set; }
    public double PassRate { get; set; }
    public int HighestScore { get; set; }
    public int LowestScore { get; set; }
    public List<QuestionAnalyticsDto> QuestionAnalytics { get; set; } = new();
}

public class QuestionAnalyticsDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int TotalAnswers { get; set; }
    public int CorrectAnswers { get; set; }
    public double CorrectRate { get; set; }
    public bool IsHardest { get; set; }
}
