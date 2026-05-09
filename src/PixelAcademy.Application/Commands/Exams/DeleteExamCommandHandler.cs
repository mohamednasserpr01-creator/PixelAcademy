using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Exams;

public class DeleteExamCommandHandler : IRequestHandler<DeleteExamCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteExamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteExamCommand request, CancellationToken cancellationToken)
    {
        var exam = await _unitOfWork.Exams.GetByIdAsync(request.Id, cancellationToken);
        if (exam == null) throw new NotFoundException("Exam", request.Id);
        if (exam.CreatedById != request.DeletedById)
            throw new ForbiddenException("You can only delete your own exams.");

        await _unitOfWork.Exams.DeleteAsync(exam, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
