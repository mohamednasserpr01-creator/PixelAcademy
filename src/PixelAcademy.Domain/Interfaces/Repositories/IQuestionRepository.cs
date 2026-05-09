using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IQuestionRepository : IRepository<Question>
{
    Task<IReadOnlyList<Question>> GetByExamAsync(Guid examId, CancellationToken cancellationToken = default);
    Task<Question?> GetWithOptionsAsync(Guid questionId, CancellationToken cancellationToken = default);
}
