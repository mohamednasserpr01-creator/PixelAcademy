using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Assignments;

public class UpdateAssignmentCommandHandler : IRequestHandler<UpdateAssignmentCommand, AssignmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AssignmentDto> Handle(UpdateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(request.Id, cancellationToken);
        if (assignment == null) throw new NotFoundException("Assignment", request.Id);
        if (assignment.CreatedById != request.UpdatedById)
            throw new ForbiddenException("You can only edit your own assignments.");

        assignment.Title = request.Title;
        assignment.Description = request.Description;
        assignment.Instructions = request.Instructions;
        assignment.DueDate = request.DueDate;
        assignment.MaxPoints = request.MaxPoints;
        assignment.AllowLateSubmission = request.AllowLateSubmission;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<AssignmentDto>(assignment);
    }
}
