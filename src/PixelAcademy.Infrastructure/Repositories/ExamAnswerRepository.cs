using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class ExamAnswerRepository : Repository<ExamAnswer>, IExamAnswerRepository
{
    public ExamAnswerRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ExamAnswer>> GetByAttemptAsync(Guid attemptId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.Question)
                .ThenInclude(q => q.Options)
            .Where(a => a.ExamAttemptId == attemptId)
            .ToListAsync(cancellationToken);
    }
}
