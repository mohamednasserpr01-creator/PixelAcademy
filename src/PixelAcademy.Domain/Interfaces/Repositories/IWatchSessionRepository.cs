using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IWatchSessionRepository : IRepository<WatchSession>
{
    Task<IReadOnlyList<WatchSession>> GetByStudentAndLectureAsync(Guid studentId, Guid lectureId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WatchSession>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<WatchSession?> GetLatestByStudentAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<int> GetTotalWatchTimeByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default);
}
