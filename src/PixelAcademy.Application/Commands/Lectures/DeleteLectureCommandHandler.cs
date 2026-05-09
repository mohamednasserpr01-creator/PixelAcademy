using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Lectures;

public class DeleteLectureCommandHandler : IRequestHandler<DeleteLectureCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DeleteLectureCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(DeleteLectureCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.Id, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.Id);

        lecture.IsDeleted = true;
        lecture.DeletedAt = _dateTimeProvider.UtcNow;
        await _unitOfWork.Lectures.UpdateAsync(lecture, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
