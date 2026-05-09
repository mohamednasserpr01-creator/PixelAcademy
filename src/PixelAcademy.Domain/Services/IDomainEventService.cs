using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Domain.Services;

public interface IDomainEventService
{
    Task PublishAsync(object domainEvent, CancellationToken cancellationToken = default);
}
