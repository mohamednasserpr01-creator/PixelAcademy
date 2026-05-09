using System;
using System.Collections.Generic;

namespace PixelAcademy.Application.DTOs.Admin;

public class AdminDashboardDto
{
    public UserStatsDto UserStats { get; set; } = new();
    public CourseStatsDto CourseStats { get; set; } = new();
    public EnrollmentStatsDto EnrollmentStats { get; set; } = new();
    public RevenueStatsDto RevenueStats { get; set; } = new();
    public ExamStatsDto ExamStats { get; set; } = new();
    public AssignmentStatsDto AssignmentStats { get; set; } = new();
    public List<ActiveStudentDto> MostActiveStudents { get; set; } = new();
    public List<PopularCourseDto> MostWatchedCourses { get; set; } = new();
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
}

public class UserStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int NewUsersToday { get; set; }
    public int NewUsersThisWeek { get; set; }
    public int BannedUsers { get; set; }
    public int Students { get; set; }
    public int Instructors { get; set; }
    public int Admins { get; set; }
}

public class CourseStatsDto
{
    public int TotalCourses { get; set; }
    public int PublishedCourses { get; set; }
    public int DraftCourses { get; set; }
    public int DisabledCourses { get; set; }
    public int TotalLectures { get; set; }
    public int TotalExams { get; set; }
    public int TotalAssignments { get; set; }
}

public class EnrollmentStatsDto
{
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public int EnrollmentsToday { get; set; }
    public int EnrollmentsThisWeek { get; set; }
    public double AverageProgressPercent { get; set; }
}

public class RevenueStatsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal RevenueToday { get; set; }
    public decimal RevenueThisWeek { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public int TotalTransactions { get; set; }
    public int RedemptionsToday { get; set; }
}

public class ExamStatsDto
{
    public int TotalExams { get; set; }
    public int TotalAttempts { get; set; }
    public int CompletedAttempts { get; set; }
    public double AverageScore { get; set; }
    public double PassRate { get; set; }
}

public class AssignmentStatsDto
{
    public int TotalAssignments { get; set; }
    public int TotalSubmissions { get; set; }
    public int GradedSubmissions { get; set; }
    public double CompletionRate { get; set; }
    public double AverageScore { get; set; }
}

public class ActiveStudentDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int WatchTimeSeconds { get; set; }
    public int CompletedLectures { get; set; }
    public int ExamAttempts { get; set; }
    public int AssignmentsSubmitted { get; set; }
    public DateTime? LastActivityAt { get; set; }
}

public class PopularCourseDto
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public int TotalWatchTimeSeconds { get; set; }
    public int LectureCount { get; set; }
    public double AverageProgressPercent { get; set; }
}

public class RecentActivityDto
{
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
