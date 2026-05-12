using MediatR;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces; // مسار الـ IUnitOfWork بتاعك

namespace PixelAcademy.Application.Commands.EducationalStages;

public class CreateEducationalStageCommandHandler : IRequestHandler<CreateEducationalStageCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEducationalStageCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateEducationalStageCommand request, CancellationToken cancellationToken)
    {
        var stage = new EducationalStage
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            IsActive = true
        };

        // هنضيف الصف في الداتا بيز
        // (لو السطر ده جاب إيرور هنظبطه مع الـ IUnitOfWork بتاعك)
        await _unitOfWork.EducationalStages.AddAsync(stage, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return stage.Id;
    }
}