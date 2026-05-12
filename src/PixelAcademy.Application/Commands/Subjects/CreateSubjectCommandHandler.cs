using MediatR;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Commands.Subjects;

public class CreateSubjectCommandHandler : IRequestHandler<CreateSubjectCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubjectCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = new Subject { Name = request.Name };
        
        await _unitOfWork.Subjects.AddAsync(subject);
        await _unitOfWork.SaveChangesAsync();

        return subject.Id;
    }
}