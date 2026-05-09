using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class LectureRepository : Repository<Lecture>, ILectureRepository
{
    public LectureRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Lecture>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.CourseId == courseId && !l.IsDeleted)
            .OrderBy(l => l.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMaxOrderIndexAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var max = await _dbSet
            .Where(l => l.CourseId == courseId && !l.IsDeleted)
            .MaxAsync(l => (int?)l.OrderIndex, cancellationToken);
        return max ?? 0;
    }
}
