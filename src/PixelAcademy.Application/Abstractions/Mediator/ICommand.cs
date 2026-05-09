using MediatR;

namespace PixelAcademy.Application.Abstractions.Mediator;

public interface ICommand<out TResponse> : IRequest<TResponse> { }
public interface ICommand : IRequest { }
