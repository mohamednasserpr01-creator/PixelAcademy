using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class AssignmentSubmissionRepository : Repository<AssignmentSubmission>, IAssignmentSubmissionRepository
{
    public AssignmentSubmissionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<AssignmentSubmission?> GetByStudentAndAssignmentAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Assignment)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.StudentId == studentId && s.AssignmentId == assignmentId, cancellationToken);
    }

    public async Task<IReadOnlyList<AssignmentSubmission>> GetByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Student)
            .Include(s => s.Assignment)
            .Where(s => s.AssignmentId == assignmentId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync(cancellationToken);
    }
}
