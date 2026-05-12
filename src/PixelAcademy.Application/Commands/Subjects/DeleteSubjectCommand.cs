using MediatR;
using PixelAcademy.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Commands.Subjects;

public class DeleteSubjectCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteSubjectCommandHandler : IRequestHandler<DeleteSubjectCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteSubjectCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<bool> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await _unitOfWork.Subjects.GetByIdAsync(request.Id, cancellationToken);
        if (subject == null) return false;
        
        // 🚀 استخدمنا DeleteAsync
        await _unitOfWork.Subjects.DeleteAsync(subject, cancellationToken); 
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}