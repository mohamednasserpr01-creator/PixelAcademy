using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.ContentItems;
using PixelAcademy.Domain.Entities;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.ContentItems;

public class CreateContentItemCommandHandler : IRequestHandler<CreateContentItemCommand, ContentItemDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateContentItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ContentItemDto> Handle(CreateContentItemCommand request, CancellationToken cancellationToken)
    {
        var lecture = await _unitOfWork.Lectures.GetByIdAsync(request.LectureId, cancellationToken);
        if (lecture == null) throw new NotFoundException("Lecture", request.LectureId);

        if (request.MediaAssetId.HasValue)
        {
            var media = await _unitOfWork.MediaAssets.GetByIdAsync(request.MediaAssetId.Value, cancellationToken);
            if (media == null) throw new NotFoundException("MediaAsset", request.MediaAssetId.Value);
        }

        var item = new ContentItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            OrderIndex = request.OrderIndex,
            Type = request.Type,
            IsRequired = request.IsRequired,
            DurationSeconds = request.DurationSeconds,
            ExternalUrl = request.ExternalUrl,
            LectureId = request.LectureId,
            MediaAssetId = request.MediaAssetId,
            CreatedAt = _dateTimeProvider.UtcNow
        };

        await _unitOfWork.ContentItems.AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContentItemDto>(item);
    }
}
