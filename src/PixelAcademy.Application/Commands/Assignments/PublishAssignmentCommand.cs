using MediatR;
using PixelAcademy.Application.DTOs.Assignments;

namespace PixelAcademy.Application.Commands.Assignments;

public record PublishAssignmentCommand(Guid Id, bool IsPublished, Guid UpdatedById) : IRequest<AssignmentDto>;
