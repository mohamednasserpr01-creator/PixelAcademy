using MediatR;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Auth;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LogoutCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.Token))
        {
            var session = await _unitOfWork.Sessions.GetByTokenAsync(request.Token, cancellationToken);
            if (session != null)
            {
                session.RevokedAt = _dateTimeProvider.UtcNow;
                await _unitOfWork.Sessions.UpdateAsync(session, cancellationToken);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
