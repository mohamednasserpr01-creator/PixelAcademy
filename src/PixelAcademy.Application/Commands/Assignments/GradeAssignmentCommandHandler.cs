using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.Assignments;

public class GradeAssignmentCommandHandler : IRequestHandler<GradeAssignmentCommand, AssignmentSubmissionDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GradeAssignmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AssignmentSubmissionDto> Handle(GradeAssignmentCommand request, CancellationToken cancellationToken)
    {
        var submission = await _unitOfWork.AssignmentSubmissions.GetByIdAsync(request.SubmissionId, cancellationToken);
        if (submission == null) throw new NotFoundException("Submission", request.SubmissionId);

        var assignment = await _unitOfWork.Assignments.GetByIdAsync(submission.AssignmentId, cancellationToken);
        if (assignment == null) throw new NotFoundException("Assignment", submission.AssignmentId);

        if (request.Score < 0 || request.Score > assignment.MaxPoints)
            throw new BadRequestException($"Score must be between 0 and {assignment.MaxPoints}.");

        submission.Score = request.Score;
        submission.Feedback = request.Feedback;
        submission.GradedByName = request.GradedByName;
        submission.GradedAt = _dateTimeProvider.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<AssignmentSubmissionDto>(submission);
    }
}
