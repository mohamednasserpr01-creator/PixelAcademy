using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IExamAnswerRepository : IRepository<ExamAnswer>
{
    Task<IReadOnlyList<ExamAnswer>> GetByAttemptAsync(Guid attemptId, CancellationToken cancellationToken = default);
}
