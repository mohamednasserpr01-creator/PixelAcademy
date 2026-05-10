using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Application.DTOs.Search;

public class SearchResultDto
{
    public List<CourseSearchResultDto> Courses { get; set; } = new();
    public List<LectureSearchResultDto> Lectures { get; set; } = new();
    public List<ExamSearchResultDto> Exams { get; set; } = new();
    public List<StudentSearchResultDto> Students { get; set; } = new();
    public int TotalCount { get; set; }
}

public class CourseSearchResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public CourseLevel Level { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LectureSearchResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsPreview { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ExamSearchResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? CourseTitle { get; set; }
    public ExamType Type { get; set; }
    public bool IsPublished { get; set; }
    public int QuestionCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class StudentSearchResultDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}
