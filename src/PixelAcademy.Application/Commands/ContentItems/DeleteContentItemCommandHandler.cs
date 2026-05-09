using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.ContentItems;

public class DeleteContentItemCommandHandler : IRequestHandler<DeleteContentItemCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContentItemCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteContentItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.ContentItems.GetByIdAsync(request.Id, cancellationToken);
        if (item == null) throw new NotFoundException("ContentItem", request.Id);

        await _unitOfWork.ContentItems.DeleteAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
