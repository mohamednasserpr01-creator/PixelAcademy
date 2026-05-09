using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Exams;

public class ExamAttemptDto
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int? Score { get; set; }
    public int? TotalPoints { get; set; }
    public bool? IsPassed { get; set; }
    public int? DurationSeconds { get; set; }
    public ExamAttemptStatus Status { get; set; }
    public List<ExamAnswerDto> Answers { get; set; } = new();
}

public class ExamAnswerDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? SelectedOptionIds { get; set; }
    public List<Guid> SelectedOptionIdList { get; set; } = new();
    public string? TextAnswer { get; set; }
    public bool? IsCorrect { get; set; }
    public int? PointsEarned { get; set; }
    public int Points { get; set; }
    public string? Explanation { get; set; }
    public List<QuestionOptionDetailDto> Options { get; set; } = new();
}

public class StartExamAttemptRequestDto
{
    public Guid ExamId { get; set; }
}

public class SubmitAnswerRequestDto
{
    public Guid QuestionId { get; set; }
    public string? SelectedOptionId { get; set; }
    public List<string>? SelectedOptionIds { get; set; }
    public string? TextAnswer { get; set; }
}

public class SubmitExamAttemptRequestDto
{
    public Guid ExamAttemptId { get; set; }
    public List<SubmitAnswerRequestDto> Answers { get; set; } = new();
}

public class ExamResultDto
{
    public Guid AttemptId { get; set; }
    public string ExamTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public int TotalPoints { get; set; }
    public double Percentage { get; set; }
    public bool IsPassed { get; set; }
    public int PassScorePercent { get; set; }
    public int DurationSeconds { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public bool ShowCorrectAnswers { get; set; }
    public List<ExamAnswerResultDto> AnswerResults { get; set; } = new();
}

public class ExamAnswerResultDto
{
    public Guid QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public int Points { get; set; }
    public int PointsEarned { get; set; }
    public bool IsCorrect { get; set; }
    public string? StudentAnswer { get; set; }
    public string? CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public List<QuestionOptionDetailDto> Options { get; set; } = new();
}
