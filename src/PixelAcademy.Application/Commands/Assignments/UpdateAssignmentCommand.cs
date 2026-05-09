using MediatR;
using PixelAcademy.Application.DTOs.Assignments;

namespace PixelAcademy.Application.Commands.Assignments;

public record UpdateAssignmentCommand(
    Guid Id,
    string Title,
    string? Description,
    string? Instructions,
    DateTime? DueDate,
    int MaxPoints,
    bool AllowLateSubmission,
    Guid UpdatedById
) : IRequest<AssignmentDto>;
