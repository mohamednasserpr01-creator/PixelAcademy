using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Assignments;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, AssignmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AssignmentDto> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        if (request.CourseId.HasValue)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId.Value, cancellationToken);
            if (course == null) throw new NotFoundException("Course", request.CourseId.Value);
            if (course.InstructorId != request.CreatedById)
                throw new ForbiddenException("You can only create assignments for your own courses.");
        }

        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Instructions = request.Instructions,
            CourseId = request.CourseId,
            LectureId = request.LectureId,
            DueDate = request.DueDate,
            MaxPoints = request.MaxPoints,
            AllowLateSubmission = request.AllowLateSubmission,
            CreatedById = request.CreatedById,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.Assignments.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AssignmentDto>(assignment);
    }
}
