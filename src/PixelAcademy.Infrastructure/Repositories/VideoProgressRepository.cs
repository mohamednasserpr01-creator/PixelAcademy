using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class VideoProgressRepository : Repository<VideoProgress>, IVideoProgressRepository
{
    public VideoProgressRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<IReadOnlyList<VideoProgress>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(vp => vp.Lecture)
            .ToListAsync(cancellationToken);
    }

    public async Task<VideoProgress?> GetByStudentAndLectureAsync(Guid studentId, Guid lectureId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(vp => vp.Lecture)
            .FirstOrDefaultAsync(vp => vp.StudentId == studentId && vp.LectureId == lectureId, cancellationToken);
    }

    public async Task<IReadOnlyList<VideoProgress>> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(vp => vp.Lecture)
            .Where(vp => vp.StudentId == studentId && vp.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VideoProgress>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(vp => vp.Student)
            .Where(vp => vp.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }
}
