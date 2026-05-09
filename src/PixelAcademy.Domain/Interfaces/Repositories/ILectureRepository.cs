using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface ILectureRepository : IRepository<Lecture>
{
    Task<IReadOnlyList<Lecture>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<int> GetMaxOrderIndexAsync(Guid courseId, CancellationToken cancellationToken = default);
}
