using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class WatchSessionRepository : Repository<WatchSession>, IWatchSessionRepository
{
    public WatchSessionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<WatchSession>> GetByStudentAndLectureAsync(Guid studentId, Guid lectureId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ws => ws.Lecture)
                .ThenInclude(l => l.Course)
            .Where(ws => ws.StudentId == studentId && ws.LectureId == lectureId)
            .OrderByDescending(ws => ws.StartedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WatchSession>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(ws => ws.Lecture)
            .Where(ws => ws.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<WatchSession?> GetLatestByStudentAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ws => ws.Lecture)
                .ThenInclude(l => l.Course)
            .Where(ws => ws.StudentId == studentId)
            .OrderByDescending(ws => ws.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetTotalWatchTimeByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ws => ws.LectureId == lectureId)
            .SumAsync(ws => ws.DurationWatchedSeconds, cancellationToken);
    }
}
