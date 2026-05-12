using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Subjects;
using PixelAcademy.Application.Queries.Subjects;

namespace PixelAcademy.API.Controllers;

[Route("api/subjects")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SubjectsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id = id, message = "تمت إضافة المادة بنجاح" });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubjectCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        if (!result) return NotFound(new { message = "المادة غير موجودة" });
        return Ok(new { message = "تم التعديل بنجاح" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteSubjectCommand { Id = id });
        if (!result) return NotFound(new { message = "المادة غير موجودة" });
        return Ok(new { message = "تم الحذف بنجاح" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subjects = await _mediator.Send(new GetAllSubjectsQuery());
        return Ok(subjects);
    }
}