using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class ExamAttemptRepository : Repository<ExamAttempt>, IExamAttemptRepository
{
    public ExamAttemptRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ExamAttempt>> GetByStudentAndExamAsync(Guid studentId, Guid examId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Exam)
            .Where(a => a.StudentId == studentId && a.ExamId == examId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExamAttempt>> GetByExamAsync(Guid examId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Student)
            .Include(a => a.Answers)
            .Where(a => a.ExamId == examId)
            .OrderByDescending(a => a.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ExamAttempt?> GetInProgressAsync(Guid studentId, Guid examId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.StudentId == studentId && a.ExamId == examId && a.Status == ExamAttemptStatus.InProgress, cancellationToken);
    }

    public async Task<int> CountAttemptsAsync(Guid studentId, Guid examId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(a => a.StudentId == studentId && a.ExamId == examId, cancellationToken);
    }

    public async Task<ExamAttempt?> GetWithAnswersAsync(Guid attemptId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Exam)
                .ThenInclude(e => e.Questions)
                    .ThenInclude(q => q.Options)
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId, cancellationToken);
    }
}
