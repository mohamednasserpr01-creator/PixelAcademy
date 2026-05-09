using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class ExamRepository : Repository<Exam>, IExamRepository
{
    public ExamRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Exam>> GetByCourseAsync(Guid courseId, bool includeUnpublished = false, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(e => e.Questions)
                .ThenInclude(q => q.Options)
            .Where(e => e.CourseId == courseId);
        if (!includeUnpublished)
            query = query.Where(e => e.IsPublished);
        return await query.OrderBy(e => e.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Exam>> GetByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Questions)
                .ThenInclude(q => q.Options)
            .Where(e => e.LectureId == lectureId && e.IsPublished)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Exam?> GetWithQuestionsAsync(Guid examId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Questions)
                .ThenInclude(q => q.Options)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == examId, cancellationToken);
    }

    public async Task<bool> IsPublishedAsync(Guid examId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.Id == examId && e.IsPublished, cancellationToken);
    }
}
