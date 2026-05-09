using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class ActivationCodeRepository : Repository<ActivationCode>, IActivationCodeRepository
{
    public ActivationCodeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<ActivationCode?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ac => ac.Course)
            .Include(ac => ac.Lecture)
            .Include(ac => ac.GeneratedBy)
            .Include(ac => ac.LastRedeemedBy)
            .FirstOrDefaultAsync(ac => ac.Code == code, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(ac => ac.Code == code, cancellationToken);
    }
}
