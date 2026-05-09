using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IExamAttemptRepository : IRepository<ExamAttempt>
{
    Task<IReadOnlyList<ExamAttempt>> GetByStudentAndExamAsync(Guid studentId, Guid examId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExamAttempt>> GetByExamAsync(Guid examId, CancellationToken cancellationToken = default);
    Task<ExamAttempt?> GetInProgressAsync(Guid studentId, Guid examId, CancellationToken cancellationToken = default);
    Task<int> CountAttemptsAsync(Guid studentId, Guid examId, CancellationToken cancellationToken = default);
    Task<ExamAttempt?> GetWithAnswersAsync(Guid attemptId, CancellationToken cancellationToken = default);
}
