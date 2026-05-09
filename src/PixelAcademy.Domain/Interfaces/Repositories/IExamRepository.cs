using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IExamRepository : IRepository<Exam>
{
    Task<IReadOnlyList<Exam>> GetByCourseAsync(Guid courseId, bool includeUnpublished = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Exam>> GetByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default);
    Task<Exam?> GetWithQuestionsAsync(Guid examId, CancellationToken cancellationToken = default);
    Task<bool> IsPublishedAsync(Guid examId, CancellationToken cancellationToken = default);
}
