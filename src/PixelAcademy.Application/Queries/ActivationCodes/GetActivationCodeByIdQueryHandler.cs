using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Queries.ActivationCodes;

public class GetActivationCodeByIdQueryHandler : IRequestHandler<GetActivationCodeByIdQuery, ActivationCodeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActivationCodeByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ActivationCodeDto> Handle(GetActivationCodeByIdQuery request, CancellationToken cancellationToken)
    {
        var code = await _unitOfWork.ActivationCodes.GetByIdAsync(request.Id, cancellationToken);
        if (code == null) throw new NotFoundException("ActivationCode", request.Id);
        return _mapper.Map<ActivationCodeDto>(code);
    }
}
