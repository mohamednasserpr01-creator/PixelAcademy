using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Admin;
using PixelAcademy.Application.DTOs.AuditLogs;
using PixelAcademy.Application.DTOs.Common;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Queries.Admin;

public class GetAdminDashboardQueryHandler : IRequestHandler<GetAdminDashboardQuery, AdminDashboardDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetAdminDashboardQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AdminDashboardDto> Handle(GetAdminDashboardQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var courses = await _unitOfWork.Courses.GetAllAsync(cancellationToken);
        var lectures = await _unitOfWork.Lectures.GetAllAsync(cancellationToken);
        var enrollments = await _unitOfWork.Enrollments.GetAllAsync(cancellationToken);
        var exams = await _unitOfWork.Exams.GetAllAsync(cancellationToken);
        var examAttempts = await _unitOfWork.ExamAttempts.GetAllAsync(cancellationToken);
        var assignments = await _unitOfWork.Assignments.GetAllAsync(cancellationToken);
        var submissions = await _unitOfWork.AssignmentSubmissions.GetAllAsync(cancellationToken);
        var watchSessions = await _unitOfWork.WatchSessions.GetAllAsync(cancellationToken);
        var transactions = await _unitOfWork.Transactions.GetAllAsync(cancellationToken);
        var auditLogs = await _unitOfWork.AuditLogs.GetAllAsync(cancellationToken);

        var today = _dateTimeProvider.UtcNow.Date;
        var weekAgo = today.AddDays(-7);

        var dashboard = new AdminDashboardDto
        {
            UserStats = new UserStatsDto
            {
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.IsActive && !u.IsBanned),
                BannedUsers = users.Count(u => u.IsBanned),
                Students = users.Count(u => u.Role == UserRole.Student),
                Instructors = users.Count(u => u.Role == UserRole.Instructor),
                Admins = users.Count(u => u.Role == UserRole.Admin),
                NewUsersToday = users.Count(u => u.CreatedAt.Date == today),
                NewUsersThisWeek = users.Count(u => u.CreatedAt >= weekAgo)
            },
            CourseStats = new CourseStatsDto
            {
                TotalCourses = courses.Count,
                PublishedCourses = courses.Count(c => c.Status == CourseStatus.Published),
                DraftCourses = courses.Count(c => c.Status == CourseStatus.Draft),
                DisabledCourses = courses.Count(c => c.IsDisabled),
                TotalLectures = lectures.Count,
                TotalExams = exams.Count,
                TotalAssignments = assignments.Count
            },
            EnrollmentStats = new EnrollmentStatsDto
            {
                TotalEnrollments = enrollments.Count,
                ActiveEnrollments = enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                CompletedEnrollments = enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
                EnrollmentsToday = enrollments.Count(e => e.CreatedAt.Date == today),
                EnrollmentsThisWeek = enrollments.Count(e => e.CreatedAt >= weekAgo),
                AverageProgressPercent = enrollments.Any() ? Math.Round(enrollments.Average(e => (double)e.ProgressPercent), 2) : 0
            },
            RevenueStats = new RevenueStatsDto
            {
                TotalRevenue = transactions.Sum(t => (decimal?)t.Amount) ?? 0m,
                RevenueToday = transactions.Where(t => t.CreatedAt.Date == today).Sum(t => (decimal?)t.Amount) ?? 0m,
                RevenueThisWeek = transactions.Where(t => t.CreatedAt >= weekAgo).Sum(t => (decimal?)t.Amount) ?? 0m,
                TotalTransactions = transactions.Count,
                RedemptionsToday = transactions.Count(t => t.CreatedAt.Date == today)
            },
            ExamStats = new ExamStatsDto
            {
                TotalExams = exams.Count,
                TotalAttempts = examAttempts.Count,
                CompletedAttempts = examAttempts.Count(a => a.Status == Domain.Enums.ExamAttemptStatus.Submitted),
                AverageScore = examAttempts.Any(a => a.Score.HasValue) ? Math.Round(examAttempts.Where(a => a.Score.HasValue).Average(a => (double)a.Score!.Value), 2) : 0,
                PassRate = examAttempts.Any(a => a.Status == Domain.Enums.ExamAttemptStatus.Submitted)
                    ? Math.Round((double)examAttempts.Count(a => a.Status == Domain.Enums.ExamAttemptStatus.Submitted && a.IsPassed == true) / examAttempts.Count(a => a.Status == Domain.Enums.ExamAttemptStatus.Submitted) * 100, 2)
                    : 0
            },
            AssignmentStats = new AssignmentStatsDto
            {
                TotalAssignments = assignments.Count,
                TotalSubmissions = submissions.Count,
                GradedSubmissions = submissions.Count(s => s.Score.HasValue),
                CompletionRate = assignments.Any() ? Math.Round((double)submissions.Count / assignments.Count * 100, 2) : 0,
                AverageScore = submissions.Any(s => s.Score.HasValue) ? Math.Round(submissions.Where(s => s.Score.HasValue).Average(s => (double)s.Score!.Value), 2) : 0
            }
        };

        // Most active students
        var studentActivity = watchSessions
            .GroupBy(ws => ws.StudentId)
            .Select(g => new
            {
                StudentId = g.Key,
                WatchTime = g.Sum(ws => ws.DurationWatchedSeconds),
                Sessions = g.Count()
            })
            .OrderByDescending(x => x.WatchTime)
            .Take(5)
            .ToList();

        dashboard.MostActiveStudents = studentActivity.Select(sa =>
        {
            var student = users.FirstOrDefault(u => u.Id == sa.StudentId);
            return new ActiveStudentDto
            {
                StudentId = sa.StudentId,
                StudentName = student != null ? student.FullName : "Unknown",
                PhoneNumber = student?.PhoneNumber ?? "",
                WatchTimeSeconds = sa.WatchTime,
                CompletedLectures = watchSessions.Count(ws => ws.StudentId == sa.StudentId && ws.IsCompleted),
                ExamAttempts = examAttempts.Count(a => a.StudentId == sa.StudentId),
                AssignmentsSubmitted = submissions.Count(s => s.StudentId == sa.StudentId),
                LastActivityAt = watchSessions.Where(ws => ws.StudentId == sa.StudentId).Max(ws => (DateTime?)ws.StartedAt)
            };
        }).ToList();

        // Most watched courses
        dashboard.MostWatchedCourses = courses
            .Select(c => new PopularCourseDto
            {
                CourseId = c.Id,
                CourseTitle = c.Title,
                InstructorName = c.Instructor != null ? c.Instructor.FullName : "Unknown",
                EnrollmentCount = enrollments.Count(e => e.CourseId == c.Id),
                TotalWatchTimeSeconds = watchSessions.Where(ws => ws.CourseId == c.Id).Sum(ws => ws.DurationWatchedSeconds),
                LectureCount = lectures.Count(l => l.CourseId == c.Id),
                AverageProgressPercent = enrollments.Any(e => e.CourseId == c.Id) ? Math.Round(enrollments.Where(e => e.CourseId == c.Id).Average(e => (double)e.ProgressPercent), 2) : 0
            })
            .OrderByDescending(c => c.EnrollmentCount)
            .Take(5)
            .ToList();

        // Recent activities from audit logs
        dashboard.RecentActivities = auditLogs
            .OrderByDescending(l => l.CreatedAt)
            .Take(10)
            .Select(l => new RecentActivityDto
            {
                ActivityType = l.Action.ToString(),
                Description = l.Details ?? $"{l.Action} on {l.EntityType}",
                UserName = l.User != null ? l.User.FullName : "System",
                Timestamp = l.CreatedAt
            })
            .ToList();

        return dashboard;
    }
}

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAuditLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var all = await _unitOfWork.AuditLogs.GetAllAsync(cancellationToken);
        var filtered = request.ActionType.HasValue
            ? all.Where(l => l.Action == request.ActionType.Value).ToList()
            : all.ToList();

        var total = filtered.Count;
        var paged = filtered
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new PagedResult<AuditLogDto>
        {
            Items = _mapper.Map<IReadOnlyList<AuditLogDto>>(paged),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}

public class GetAdminAuditLogsQueryHandler : IRequestHandler<GetAdminAuditLogsQuery, PagedResult<AuditLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAdminAuditLogsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(GetAdminAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var logs = await _unitOfWork.AuditLogs.GetAdminActionsAsync(request.Page, request.PageSize, cancellationToken);
        // Get total count from all
        var all = await _unitOfWork.AuditLogs.GetAllAsync(cancellationToken);
        var total = all.Count(l => l.IsAdminAction);

        return new PagedResult<AuditLogDto>
        {
            Items = _mapper.Map<IReadOnlyList<AuditLogDto>>(logs),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = total
        };
    }
}
