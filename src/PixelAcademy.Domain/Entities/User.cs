using System;
using System.Collections.Generic;
using PixelAcademy.Domain.Common;
using PixelAcademy.Domain.Enums;

namespace PixelAcademy.Domain.Entities;

public class User : SoftDeleteEntity
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public UserRole Role { get; set; } = UserRole.Student;
    public bool IsActive { get; set; } = true;
    public bool IsBanned { get; set; }
    public DateTime? BannedAt { get; set; }
    public string? BannedReason { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public decimal WalletBalance { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Course> OwnedCourses { get; set; } = new List<Course>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<VideoProgress> VideoProgresses { get; set; } = new List<VideoProgress>();
    public ICollection<ActivationCode> GeneratedCodes { get; set; } = new List<ActivationCode>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<LectureAccess> LectureAccesses { get; set; } = new List<LectureAccess>();
    public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    public ICollection<Exam> CreatedExams { get; set; } = new List<Exam>();
    public ICollection<ExamAttempt> ExamAttempts { get; set; } = new List<ExamAttempt>();
    public ICollection<Assignment> CreatedAssignments { get; set; } = new List<Assignment>();
    public ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Announcement> CreatedAnnouncements { get; set; } = new List<Announcement>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
