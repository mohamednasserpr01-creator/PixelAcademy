using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<IReadOnlyList<Assignment>> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<Assignment?> GetWithSubmissionsAsync(Guid assignmentId, CancellationToken cancellationToken = default);
}
