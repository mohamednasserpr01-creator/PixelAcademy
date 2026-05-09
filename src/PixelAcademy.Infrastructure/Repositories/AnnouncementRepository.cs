using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class AnnouncementRepository : Repository<Announcement>, IAnnouncementRepository
{
    public AnnouncementRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Announcement>> GetPublishedAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Course)
            .Include(a => a.Creator)
            .Where(a => a.IsPublished)
            .OrderByDescending(a => a.Priority)
            .ThenByDescending(a => a.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Announcement>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.CourseId == courseId && a.IsPublished)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Announcement>> GetScheduledAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => !a.IsPublished && a.ScheduledPublishAt.HasValue && a.ScheduledPublishAt.Value <= DateTime.UtcNow)
            .OrderBy(a => a.ScheduledPublishAt)
            .ToListAsync(cancellationToken);
    }
}
