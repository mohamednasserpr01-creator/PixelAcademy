using PixelAcademy.Application.Abstractions.Mediator;
using PixelAcademy.Application.DTOs.ActivationCodes;

namespace PixelAcademy.Application.Queries.ActivationCodes;

public class GetActivationCodeByIdQuery : IQuery<ActivationCodeDto>
{
    public Guid Id { get; set; }
}
