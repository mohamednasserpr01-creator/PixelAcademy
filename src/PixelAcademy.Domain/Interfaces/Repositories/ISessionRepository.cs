using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface ISessionRepository : IRepository<Session>
{
    Task<IReadOnlyList<Session>> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken = default);
}
