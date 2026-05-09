using AutoMapper;
using MediatR;
using PixelAcademy.Application.DTOs.ContentItems;
using PixelAcademy.Domain.Exceptions;
using PixelAcademy.Domain.Interfaces;
using PixelAcademy.Domain.Interfaces.Repositories;

namespace PixelAcademy.Application.Commands.ContentItems;

public class UpdateContentItemCommandHandler : IRequestHandler<UpdateContentItemCommand, ContentItemDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateContentItemCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ContentItemDto> Handle(UpdateContentItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.ContentItems.GetByIdAsync(request.Id, cancellationToken);
        if (item == null) throw new NotFoundException("ContentItem", request.Id);

        if (request.MediaAssetId.HasValue)
        {
            var media = await _unitOfWork.MediaAssets.GetByIdAsync(request.MediaAssetId.Value, cancellationToken);
            if (media == null) throw new NotFoundException("MediaAsset", request.MediaAssetId.Value);
        }

        if (!string.IsNullOrEmpty(request.Title)) item.Title = request.Title;
        if (request.Description != null) item.Description = request.Description;
        if (request.OrderIndex.HasValue) item.OrderIndex = request.OrderIndex.Value;
        if (request.Type.HasValue) item.Type = request.Type.Value;
        if (request.IsRequired.HasValue) item.IsRequired = request.IsRequired.Value;
        if (request.DurationSeconds.HasValue) item.DurationSeconds = request.DurationSeconds.Value;
        if (request.ExternalUrl != null) item.ExternalUrl = request.ExternalUrl;
        if (request.MediaAssetId.HasValue) item.MediaAssetId = request.MediaAssetId;

        item.UpdatedAt = _dateTimeProvider.UtcNow;
        await _unitOfWork.ContentItems.UpdateAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ContentItemDto>(item);
    }
}
