using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IMediaAssetRepository : IRepository<MediaAsset>
{
    Task<IReadOnlyList<MediaAsset>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MediaAsset>> GetByLectureAsync(Guid lectureId, CancellationToken cancellationToken = default);
}
