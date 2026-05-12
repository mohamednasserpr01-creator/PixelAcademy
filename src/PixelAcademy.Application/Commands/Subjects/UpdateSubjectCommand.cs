using MediatR;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Commands.Subjects;

public class UpdateSubjectCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class UpdateSubjectCommandHandler : IRequestHandler<UpdateSubjectCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public UpdateSubjectCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _unitOfWork.Subjects.GetByIdAsync(request.Id);
        if (subject == null) return false;
        
        subject.Name = request.Name;
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}