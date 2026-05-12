using System;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICourseRepository Courses { get; }
    ILectureRepository Lectures { get; }
    IEnrollmentRepository Enrollments { get; }
    ISessionRepository Sessions { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IVideoProgressRepository VideoProgresses { get; }
    IMediaAssetRepository MediaAssets { get; }
    IContentItemRepository ContentItems { get; }
    IActivationCodeRepository ActivationCodes { get; }
    ITransactionRepository Transactions { get; }
    ILectureAccessRepository LectureAccesses { get; }
    IWalletTransactionRepository WalletTransactions { get; }
    IWatchSessionRepository WatchSessions { get; }
    IExamRepository Exams { get; }
    IQuestionRepository Questions { get; }
    IExamAttemptRepository ExamAttempts { get; }
    IExamAnswerRepository ExamAnswers { get; }
    IAssignmentRepository Assignments { get; }
    IAssignmentSubmissionRepository AssignmentSubmissions { get; }
    INotificationRepository Notifications { get; }
    IAnnouncementRepository Announcements { get; }
    IAuditLogRepository AuditLogs { get; }

    // 🚀 الجداول الجديدة
    IEducationalStageRepository EducationalStages { get; }
    IEducationStreamRepository EducationStreams { get; }
    ISubjectRepository Subjects { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}