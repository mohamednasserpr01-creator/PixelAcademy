using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Media;
using PixelAcademy.Application.DTOs.Media;
using PixelAcademy.Application.Queries.Media;
using PixelAcademy.Domain.Enums;
using PixelAcademy.Domain.Interfaces;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public MediaController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet("course/{courseId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<MediaAssetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MediaAssetDto>>> GetByCourse(Guid courseId)
    {
        var result = await _mediator.Send(new GetMediaByCourseQuery { CourseId = courseId });
        return Ok(result);
    }

    [HttpGet("lecture/{lectureId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<MediaAssetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MediaAssetDto>>> GetByLecture(Guid lectureId)
    {
        var result = await _mediator.Send(new GetMediaByLectureQuery { LectureId = lectureId });
        return Ok(result);
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(MediaAssetDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<MediaAssetDto>> UploadFile(
        [FromForm] IFormFile file,
        [FromForm] Guid? courseId,
        [FromForm] Guid? lectureId)
    {
        // File upload validation
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided." });

        var maxSize = 100 * 1024 * 1024; // 100 MB
        if (file.Length > maxSize)
            return BadRequest(new { message = $"File size exceeds maximum allowed ({maxSize / 1024 / 1024} MB)." });

        var allowedTypes = new[] { "video/mp4", "video/webm", "video/ogg", "audio/mpeg", "audio/wav", "application/pdf", "image/jpeg", "image/png", "image/gif" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { message = $"File type '{file.ContentType}' is not supported." });

        // In production, save to cloud storage and return URL
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var url = $"/uploads/{fileName}";

        var result = await _mediator.Send(new UploadMediaCommand
        {
            FileName = fileName,
            OriginalFileName = file.FileName,
            Url = url,
            Type = file.ContentType.StartsWith("video") ? MediaType.Video : file.ContentType.StartsWith("audio") ? MediaType.Audio : file.ContentType == "application/pdf" ? MediaType.Document : MediaType.Image,
            FileSize = file.Length,
            MimeType = file.ContentType,
            CourseId = courseId,
            LectureId = lectureId,
            UploadedById = _currentUserService.UserId!.Value
        });
        return CreatedAtAction(nameof(GetByCourse), new { courseId = courseId ?? Guid.Empty }, result);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteMediaCommand { Id = id });
        return NoContent();
    }
}
