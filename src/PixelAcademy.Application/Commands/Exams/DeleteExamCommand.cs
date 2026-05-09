using MediatR;

namespace PixelAcademy.Application.Commands.Exams;

public record DeleteExamCommand(Guid Id, Guid DeletedById) : IRequest<bool>;
