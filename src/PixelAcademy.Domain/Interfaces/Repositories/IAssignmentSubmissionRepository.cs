using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IAssignmentSubmissionRepository : IRepository<AssignmentSubmission>
{
    Task<AssignmentSubmission?> GetByStudentAndAssignmentAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AssignmentSubmission>> GetByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);
}
