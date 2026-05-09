using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class WalletTransactionRepository : Repository<WalletTransaction>, IWalletTransactionRepository
{
    public WalletTransactionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IReadOnlyList<WalletTransaction>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(wt => wt.ActivationCode)
            .Include(wt => wt.Course)
            .Include(wt => wt.Lecture)
            .Where(wt => wt.UserId == userId)
            .OrderByDescending(wt => wt.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
