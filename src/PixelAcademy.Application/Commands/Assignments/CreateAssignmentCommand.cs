using MediatR;
using PixelAcademy.Application.DTOs.Assignments;

namespace PixelAcademy.Application.Commands.Assignments;

public record CreateAssignmentCommand(
    string Title,
    string? Description,
    string? Instructions,
    Guid? CourseId,
    Guid? LectureId,
    DateTime? DueDate,
    int MaxPoints,
    bool AllowLateSubmission,
    Guid CreatedById
) : IRequest<AssignmentDto>;
