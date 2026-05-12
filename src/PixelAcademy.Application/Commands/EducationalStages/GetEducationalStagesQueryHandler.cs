using MediatR;
using Microsoft.EntityFrameworkCore;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.Application.Commands.EducationalStages;

public class GetEducationalStagesQueryHandler : IRequestHandler<GetEducationalStagesQuery, List<EducationalStage>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetEducationalStagesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<EducationalStage>> Handle(GetEducationalStagesQuery request, CancellationToken cancellationToken)
    {
        // بنجيب المراحل ونجيب معاها الشعب بتاعتها
        var stages = await _unitOfWork.EducationalStages.AsQueryable()
            .Include(e => e.EducationStreams)
            .ToListAsync(cancellationToken);
            
        return stages;
    }
}