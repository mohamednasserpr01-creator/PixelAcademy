using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Assignments;

public class DeleteAssignmentCommandHandler : IRequestHandler<DeleteAssignmentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAssignmentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(request.Id, cancellationToken);
        if (assignment == null) throw new NotFoundException("Assignment", request.Id);
        if (assignment.CreatedById != request.DeletedById)
            throw new ForbiddenException("You can only delete your own assignments.");

        await _unitOfWork.Assignments.DeleteAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
