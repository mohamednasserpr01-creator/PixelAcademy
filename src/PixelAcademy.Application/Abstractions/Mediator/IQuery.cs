using MediatR;

namespace PixelAcademy.Application.Abstractions.Mediator;

public interface IQuery<out TResponse> : IRequest<TResponse> { }
