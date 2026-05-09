using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.ActivationCodes;

public class DisableActivationCodeCommandHandler : IRequestHandler<DisableActivationCodeCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DisableActivationCodeCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DisableActivationCodeCommand request, CancellationToken cancellationToken)
    {
        var code = await _unitOfWork.ActivationCodes.GetByIdAsync(request.Id, cancellationToken);
        if (code == null) throw new NotFoundException("ActivationCode", request.Id);

        if (code.GeneratedById != request.RequestedById)
            throw new ForbiddenException("You can only disable codes you generated.");

        code.IsActive = false;
        await _unitOfWork.ActivationCodes.UpdateAsync(code, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
