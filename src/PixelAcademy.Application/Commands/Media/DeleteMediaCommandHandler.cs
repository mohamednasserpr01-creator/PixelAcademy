using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Media;

public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMediaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await _unitOfWork.MediaAssets.GetByIdAsync(request.Id, cancellationToken);
        if (media == null) throw new NotFoundException("MediaAsset", request.Id);

        await _unitOfWork.MediaAssets.DeleteAsync(media, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
