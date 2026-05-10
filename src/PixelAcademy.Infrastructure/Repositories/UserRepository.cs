using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces.Repositories;
using PixelAcademy.Infrastructure.Data;

namespace PixelAcademy.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted, cancellationToken);
    }

    public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return !await _dbSet.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default)
    {
        return !await _dbSet.AnyAsync(u => u.Username == username, cancellationToken);
    }
}
