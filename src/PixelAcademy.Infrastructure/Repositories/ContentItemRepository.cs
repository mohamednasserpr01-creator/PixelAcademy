using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class ContentItemRepository : Repository<ContentItem>, IContentItemRepository
{
    public ContentItemRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ContentItem>> GetByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(ci => ci.LectureId == lectureId && !ci.IsDeleted)
            .OrderBy(ci => ci.OrderIndex)
            .Include(ci => ci.MediaAsset)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetMaxOrderIndexAsync(Guid lectureId, CancellationToken cancellationToken = default)
    {
        var max = await _dbSet
            .Where(ci => ci.LectureId == lectureId && !ci.IsDeleted)
            .MaxAsync(ci => (int?)ci.OrderIndex, cancellationToken);
        return max ?? 0;
    }
}
