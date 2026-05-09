using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class SessionRepository : Repository<Session>, ISessionRepository
{
    public SessionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Session>> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.UserId == userId && !s.IsRevoked && !s.IsExpired)
            .ToListAsync(cancellationToken);
    }

    public async Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Token == token, cancellationToken);
    }

    public async Task RevokeAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var sessions = await _dbSet.Where(s => s.UserId == userId && !s.IsRevoked).ToListAsync(cancellationToken);
        foreach (var session in sessions)
        {
            session.RevokedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(sessions);
    }
}
