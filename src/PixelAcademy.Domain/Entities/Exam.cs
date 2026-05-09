using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class Exam : SoftDeleteEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ExamType Type { get; set; } = ExamType.CourseExam;
    public Guid? CourseId { get; set; }
    public Course? Course { get; set; }
    public Guid? LectureId { get; set; }
    public Lecture? Lecture { get; set; }
    public int? DurationMinutes { get; set; }
    public int AttemptLimit { get; set; } = 1;
    public int PassScorePercent { get; set; } = 50;
    public bool IsPublished { get; set; }
    public bool IsRandomized { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool ShowCorrectAnswers { get; set; }
    public Guid CreatedById { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<ExamAttempt> Attempts { get; set; } = new List<ExamAttempt>();
}
