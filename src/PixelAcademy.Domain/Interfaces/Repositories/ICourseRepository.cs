using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default);
}
