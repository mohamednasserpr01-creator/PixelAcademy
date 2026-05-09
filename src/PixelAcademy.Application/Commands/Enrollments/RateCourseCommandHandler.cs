using MediatR;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.Enrollments;

public class RateCourseCommandHandler : IRequestHandler<RateCourseCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RateCourseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RateCourseCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.EnrollmentId, cancellationToken);
        if (enrollment == null) throw new NotFoundException("Enrollment", request.EnrollmentId);

        if (request.Rating < 1 || request.Rating > 5)
            throw new BadRequestException("Rating must be between 1 and 5.");

        enrollment.Rating = request.Rating;
        enrollment.Review = request.Review;
        await _unitOfWork.Enrollments.UpdateAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
