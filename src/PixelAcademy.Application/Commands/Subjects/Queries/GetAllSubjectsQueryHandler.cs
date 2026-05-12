using MediatR;
using PixelAcademy.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PixelAcademy.Application.Queries.Subjects;

public class GetAllSubjectsQueryHandler : IRequestHandler<GetAllSubjectsQuery, IEnumerable<SubjectDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllSubjectsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SubjectDto>> Handle(GetAllSubjectsQuery request, CancellationToken cancellationToken)
    {
        var subjects = await _unitOfWork.Subjects.GetAllAsync();
        
        return subjects.Select(s => new SubjectDto 
        { 
            Id = s.Id, 
            Name = s.Name 
        });
    }
}