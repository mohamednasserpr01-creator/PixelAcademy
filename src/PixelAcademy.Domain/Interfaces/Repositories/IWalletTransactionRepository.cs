using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PixelAcademy.Domain.Entities;

namespace PixelAcademy.Domain.Interfaces.Repositories;

public interface IWalletTransactionRepository : IRepository<WalletTransaction>
{
    Task<IReadOnlyList<WalletTransaction>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
