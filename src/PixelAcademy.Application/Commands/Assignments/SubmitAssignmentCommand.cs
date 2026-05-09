using MediatR;
using PixelAcademy.Application.DTOs.Assignments;

namespace PixelAcademy.Application.Commands.Assignments;

public record SubmitAssignmentCommand(
    Guid AssignmentId,
    Guid StudentId,
    string? TextAnswer,
    string? FileUrl
) : IRequest<AssignmentSubmissionDto>;
