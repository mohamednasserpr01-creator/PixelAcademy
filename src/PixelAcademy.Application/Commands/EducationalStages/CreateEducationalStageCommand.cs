using MediatR;

namespace PixelAcademy.Application.Commands.EducationalStages;

public class CreateEducationalStageCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
}