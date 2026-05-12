using MediatR;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Commands.EducationalStages;

// --- حذف صف دراسي ---
public class DeleteEducationalStageCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteEducationalStageCommandHandler : IRequestHandler<DeleteEducationalStageCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteEducationalStageCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteEducationalStageCommand request, CancellationToken cancellationToken)
    {
        var stage = await _unitOfWork.EducationalStages.GetByIdAsync(request.Id, cancellationToken);
        if (stage == null) return false;
        
        // 🚀 استخدمنا DeleteAsync
        await _unitOfWork.EducationalStages.DeleteAsync(stage, cancellationToken); 
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

// --- حذف شعبة ---
public class DeleteEducationStreamCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteEducationStreamCommandHandler : IRequestHandler<DeleteEducationStreamCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteEducationStreamCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteEducationStreamCommand request, CancellationToken cancellationToken)
    {
        var stream = await _unitOfWork.EducationStreams.GetByIdAsync(request.Id, cancellationToken);
        if (stream == null) return false;
        
        // 🚀 استخدمنا DeleteAsync
        await _unitOfWork.EducationStreams.DeleteAsync(stream, cancellationToken); 
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}