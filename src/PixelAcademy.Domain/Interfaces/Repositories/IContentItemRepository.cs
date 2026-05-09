using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IContentItemRepository : IRepository<ContentItem>
{
    Task<IReadOnlyList<ContentItem>> GetByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderIndexAsync(Guid lectureId, CancellationToken cancellationToken = default);
}
