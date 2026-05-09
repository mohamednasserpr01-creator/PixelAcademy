using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Assignments;

public class PublishAssignmentCommandHandler : IRequestHandler<PublishAssignmentCommand, AssignmentDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PublishAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AssignmentDto> Handle(PublishAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(request.Id, cancellationToken);
        if (assignment == null) throw new NotFoundException("Assignment", request.Id);
        if (assignment.CreatedById != request.UpdatedById)
            throw new ForbiddenException("You can only publish your own assignments.");

        assignment.IsPublished = request.IsPublished;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<AssignmentDto>(assignment);
    }
}
