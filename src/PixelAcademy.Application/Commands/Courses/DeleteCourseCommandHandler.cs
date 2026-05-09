using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Courses;

public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCourseCommandHandler(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id, cancellationToken);
        if (course == null) throw new NotFoundException("Course", request.Id);

        if (course.InstructorId != request.RequestedById)
            throw new ForbiddenException("You can only delete your own courses.");

        course.IsDeleted = true;
        course.DeletedAt = _dateTimeProvider.UtcNow;
        course.DeletedBy = _currentUserService.Email ?? "system";
        await _unitOfWork.Courses.UpdateAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
