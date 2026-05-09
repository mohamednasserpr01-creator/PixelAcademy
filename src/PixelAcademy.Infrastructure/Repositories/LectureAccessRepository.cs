using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class LectureAccessRepository : Repository<LectureAccess>, ILectureAccessRepository
{
    public LectureAccessRepository(ApplicationDbContext context) : base(context) { }

    public async Task<LectureAccess?> GetByStudentAndLectureAsync(Guid studentId, Guid lectureId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(la => la.StudentId == studentId && la.LectureId == lectureId, cancellationToken);
    }

    public async Task<bool> HasAccessAsync(Guid studentId, Guid lectureId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(la => la.StudentId == studentId && la.LectureId == lectureId, cancellationToken);
    }
}
