using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface ILectureAccessRepository : IRepository<LectureAccess>
{
    Task<LectureAccess?> GetByStudentAndLectureAsync(Guid studentId, Guid lectureId, CancellationToken cancellationToken = default);
    Task<bool> HasAccessAsync(Guid studentId, Guid lectureId, CancellationToken cancellationToken = default);
}
