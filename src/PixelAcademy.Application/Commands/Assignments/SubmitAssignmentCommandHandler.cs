using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Assignments;

public class SubmitAssignmentCommandHandler : IRequestHandler<SubmitAssignmentCommand, AssignmentSubmissionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SubmitAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AssignmentSubmissionDto> Handle(SubmitAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(request.AssignmentId, cancellationToken);
        if (assignment == null) throw new NotFoundException("Assignment", request.AssignmentId);
        if (!assignment.IsPublished) throw new ForbiddenException("This assignment is not published.");

        // Validate enrollment
        if (assignment.CourseId.HasValue)
        {
            var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.StudentId, assignment.CourseId.Value, cancellationToken);
            if (!isEnrolled)
                throw new ForbiddenException("You must be enrolled in the course to submit this assignment.");
        }

        var existing = await _unitOfWork.AssignmentSubmissions.GetByStudentAndAssignmentAsync(request.StudentId, request.AssignmentId, cancellationToken);
        if (existing != null)
            throw new BadRequestException("You have already submitted this assignment.");

        var isLate = assignment.DueDate.HasValue && _dateTimeProvider.UtcNow > assignment.DueDate.Value && !assignment.AllowLateSubmission;
        if (isLate)
            throw new BadRequestException("The deadline for this assignment has passed.");

        var isLateFlag = assignment.DueDate.HasValue && _dateTimeProvider.UtcNow > assignment.DueDate.Value;

        var submission = new AssignmentSubmission
        {
            Id = Guid.NewGuid(),
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentId,
            TextAnswer = request.TextAnswer,
            FileUrl = request.FileUrl,
            SubmittedAt = _dateTimeProvider.UtcNow,
            IsLate = isLateFlag,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.AssignmentSubmissions.AddAsync(submission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AssignmentSubmissionDto>(submission);
    }
}
