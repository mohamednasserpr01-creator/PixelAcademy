using MediatR;

namespace PixelAcademy.Application.Commands.Questions;

public record DeleteQuestionCommand(Guid Id) : IRequest<bool>;
