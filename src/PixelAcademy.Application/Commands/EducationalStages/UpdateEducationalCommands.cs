using MediatR;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Commands.EducationalStages;

// --- تعديل صف دراسي ---
public class UpdateEducationalStageCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class UpdateEducationalStageCommandHandler : IRequestHandler<UpdateEducationalStageCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateEducationalStageCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateEducationalStageCommand request, CancellationToken cancellationToken)
    {
        var stage = await _unitOfWork.EducationalStages.GetByIdAsync(request.Id);
        if (stage == null) return false;
        
        stage.Name = request.Name;
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}

// --- تعديل شعبة ---
public class UpdateEducationStreamCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class UpdateEducationStreamCommandHandler : IRequestHandler<UpdateEducationStreamCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateEducationStreamCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateEducationStreamCommand request, CancellationToken cancellationToken)
    {
        var stream = await _unitOfWork.EducationStreams.GetByIdAsync(request.Id);
        if (stream == null) return false;
        
        stream.Name = request.Name;
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}