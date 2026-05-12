using MediatR;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Commands.EducationalStages;

public class CreateEducationStreamCommandHandler : IRequestHandler<CreateEducationStreamCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEducationStreamCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateEducationStreamCommand request, CancellationToken cancellationToken)
    {
        // بنعمل الشعبة الجديدة وبنربطها بـ ID الصف الدراسي
        var stream = new EducationStream 
        { 
            Name = request.Name,
            EducationalStageId = request.EducationalStageId 
        };

        // بنضيفها في الداتا بيز (لو الـ UnitOfWork عندك مش مسمي الجدول EducationStreams، هنعدلها بس ده الطبيعي)
        await _unitOfWork.EducationStreams.AddAsync(stream);
        await _unitOfWork.SaveChangesAsync();

        return stream.Id;
    }
}