using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.EducationalStages;

namespace PixelAcademy.API.Controllers;

[Route("api/educational-stages")]
[ApiController]
public class EducationalStagesController : ControllerBase
{
    private readonly IMediator _mediator;
    public EducationalStagesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEducationalStageCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id = id, message = "تمت إضافة الصف الدراسي بنجاح" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEducationalStageCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        if (!result) return NotFound(new { message = "الصف الدراسي غير موجود" });
        return Ok(new { message = "تم التعديل بنجاح" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteEducationalStageCommand { Id = id });
        if (!result) return NotFound(new { message = "الصف الدراسي غير موجود" });
        return Ok(new { message = "تم الحذف بنجاح" });
    }

    [HttpPost("{stageId}/streams")]
    public async Task<IActionResult> AddStream(Guid stageId, [FromBody] CreateEducationStreamCommand command)
    {
        command.EducationalStageId = stageId;
        var id = await _mediator.Send(command);
        return Ok(new { id = id, message = "تمت إضافة الشعبة بنجاح" });
    }

    [HttpPut("streams/{streamId}")]
    public async Task<IActionResult> UpdateStream(Guid streamId, [FromBody] UpdateEducationStreamCommand command)
    {
        command.Id = streamId;
        var result = await _mediator.Send(command);
        if (!result) return NotFound(new { message = "الشعبة غير موجودة" });
        return Ok(new { message = "تم التعديل بنجاح" });
    }

    [HttpDelete("streams/{streamId}")]
    public async Task<IActionResult> DeleteStream(Guid streamId)
    {
        var result = await _mediator.Send(new DeleteEducationStreamCommand { Id = streamId });
        if (!result) return NotFound(new { message = "الشعبة غير موجودة" });
        return Ok(new { message = "تم الحذف بنجاح" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stages = await _mediator.Send(new GetEducationalStagesQuery());
        var result = stages.Select(s => new 
        {
            id = s.Id.ToString(),
            name = s.Name,
            streams = s.EducationStreams.Select(st => new { id = st.Id.ToString(), name = st.Name }).ToList()
        });
        return Ok(result);
    }
}