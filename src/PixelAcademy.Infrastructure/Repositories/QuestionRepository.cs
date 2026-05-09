using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class QuestionRepository : Repository<Question>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Question>> GetByExamAsync(Guid examId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(q => q.Options)
            .Where(q => q.ExamId == examId)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync(cancellationToken);
    }

    public async Task<Question?> GetWithOptionsAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == questionId, cancellationToken);
    }
}
