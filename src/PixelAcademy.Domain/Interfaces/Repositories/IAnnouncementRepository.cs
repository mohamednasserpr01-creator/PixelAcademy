using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IAnnouncementRepository : IRepository<Announcement>
{
    Task<IReadOnlyList<Announcement>> GetPublishedAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Announcement>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Announcement>> GetScheduledAsync(CancellationToken cancellationToken = default);
}
