using MediatR;

namespace PixelAcademy.Application.Commands.Assignments;

public record DeleteAssignmentCommand(Guid Id, Guid DeletedById) : IRequest<bool>;
