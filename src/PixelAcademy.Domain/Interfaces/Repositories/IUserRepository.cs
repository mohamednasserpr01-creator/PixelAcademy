using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<bool> IsUsernameUniqueAsync(string username, CancellationToken cancellationToken = default);
}
