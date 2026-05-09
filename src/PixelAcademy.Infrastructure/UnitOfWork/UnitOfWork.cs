using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;
using PixelAcademy.Infrastructure.Repositories;

namespace PixelAcademy.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private bool _disposed;

    public IUserRepository Users { get; }
    public ICourseRepository Courses { get; }
    public ILectureRepository Lectures { get; }
    public IEnrollmentRepository Enrollments { get; }
    public ISessionRepository Sessions { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IVideoProgressRepository VideoProgresses { get; }
    public IMediaAssetRepository MediaAssets { get; }
    public IContentItemRepository ContentItems { get; }
    public IActivationCodeRepository ActivationCodes { get; }
    public ITransactionRepository Transactions { get; }
    public ILectureAccessRepository LectureAccesses { get; }
    public IWalletTransactionRepository WalletTransactions { get; }
    public IWatchSessionRepository WatchSessions { get; }
    public IExamRepository Exams { get; }
    public IQuestionRepository Questions { get; }
    public IExamAttemptRepository ExamAttempts { get; }
    public IExamAnswerRepository ExamAnswers { get; }
    public IAssignmentRepository Assignments { get; }
    public IAssignmentSubmissionRepository AssignmentSubmissions { get; }
    public INotificationRepository Notifications { get; }
    public IAnnouncementRepository Announcements { get; }
    public IAuditLogRepository AuditLogs { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Courses = new CourseRepository(context);
        Lectures = new LectureRepository(context);
        Enrollments = new EnrollmentRepository(context);
        Sessions = new SessionRepository(context);
        RefreshTokens = new RefreshTokenRepository(context);
        VideoProgresses = new VideoProgressRepository(context);
        MediaAssets = new MediaAssetRepository(context);
        ContentItems = new ContentItemRepository(context);
        ActivationCodes = new ActivationCodeRepository(context);
        Transactions = new TransactionRepository(context);
        LectureAccesses = new LectureAccessRepository(context);
        WalletTransactions = new WalletTransactionRepository(context);
        WatchSessions = new WatchSessionRepository(context);
        Exams = new ExamRepository(context);
        Questions = new QuestionRepository(context);
        ExamAttempts = new ExamAttemptRepository(context);
        ExamAnswers = new ExamAnswerRepository(context);
        Assignments = new AssignmentRepository(context);
        AssignmentSubmissions = new AssignmentSubmissionRepository(context);
        Notifications = new NotificationRepository(context);
        Announcements = new AnnouncementRepository(context);
        AuditLogs = new AuditLogRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
