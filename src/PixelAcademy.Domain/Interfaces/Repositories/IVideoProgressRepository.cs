using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IVideoProgressRepository : IRepository<VideoProgress>
{
    Task<VideoProgress?> GetByStudentAndLectureAsync(Guid studentId, Guid lectureId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VideoProgress>> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VideoProgress>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
}
