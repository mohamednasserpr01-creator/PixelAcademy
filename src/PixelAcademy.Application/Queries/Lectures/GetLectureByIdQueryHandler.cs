using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.Lectures;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.Lectures;

public class GetLectureByIdQueryHandler : IRequestHandler<GetLectureByIdQuery, LectureDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLectureByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LectureDto> Handle(GetLectureByIdQuery request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.Id, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.Id);
        return _mapper.Map<LectureDto>(lecture);
    }
}
