using MediatR;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Auth;

public class RevokeAllSessionsCommandHandler : IRequestHandler<RevokeAllSessionsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RevokeAllSessionsCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(RevokeAllSessionsCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.Sessions.RevokeAllAsync(request.UserId, cancellationToken);
        await _unitOfWork.RefreshTokens.RevokeAllAsync(request.UserId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
