using MediatR;
using PixelAcademy.Application.DTOs.Assignments;

namespace PixelAcademy.Application.Commands.Assignments;

public record GradeAssignmentCommand(
    Guid SubmissionId,
    int Score,
    string? Feedback,
    string GradedByName
) : IRequest<AssignmentSubmissionDto>;
