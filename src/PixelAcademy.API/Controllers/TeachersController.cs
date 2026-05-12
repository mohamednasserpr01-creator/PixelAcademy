using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Teachers;
using PixelAcademy.Application.Queries.Teachers; 

namespace PixelAcademy.API.Controllers;

[Route("api/teachers")]
[ApiController]
public class TeachersController : ControllerBase
{
    private readonly IMediator _mediator;
    public TeachersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // بيجيب كل المدرسين
        var result = await _mediator.Send(new GetAllTeachersQuery());
        return Ok(result);
    }

    // 🚀 المسار الجديد الخاص بسحب بيانات طلاب المدرس في شيت إكسيل
    [HttpGet("{id}/students")]
    public async Task<IActionResult> GetStudents(Guid id, [FromQuery] string stage = "الكل")
    {
        var result = await _mediator.Send(new GetTeacherStudentsQuery { TeacherId = id, Stage = stage });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherCommand command)
    {
        try
        {
            var id = await _mediator.Send(command);
            return Ok(new { id = id, message = "تمت إضافة المدرس بنجاح" });
        }
        catch (Exception ex)
        {
            // 🚀 اصطياد الإيرور وترجمته للعربي
            var errorMsg = ex.InnerException?.Message ?? ex.Message;
            if (errorMsg.Contains("duplicate key") || errorMsg.Contains("UNIQUE") || errorMsg.Contains("Duplicate"))
            {
                return BadRequest(new { message = "رقم الهاتف هذا مسجل بالفعل لمستخدم آخر." });
            }
            return BadRequest(new { message = "حدث خطأ في قاعدة البيانات، يرجى التأكد من صحة البيانات وعدم تكرارها." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherCommand command)
    {
        try
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            if (!result) return NotFound(new { message = "المدرس غير موجود" });
            return Ok(new { message = "تم التعديل بنجاح" });
        }
        catch (Exception ex)
        {
            var errorMsg = ex.InnerException?.Message ?? ex.Message;
            if (errorMsg.Contains("duplicate key") || errorMsg.Contains("UNIQUE") || errorMsg.Contains("Duplicate"))
            {
                return BadRequest(new { message = "رقم الهاتف الجديد مسجل بالفعل لمستخدم آخر." });
            }
            return BadRequest(new { message = "حدث خطأ أثناء تعديل البيانات." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteTeacherCommand { Id = id });
        if (!result) return NotFound(new { message = "المدرس غير موجود" });
        return Ok(new { message = "تم الحذف بنجاح" });
    }

    [HttpPatch("{id}/ban")]
    public async Task<IActionResult> Ban(Guid id)
    {
        var result = await _mediator.Send(new ToggleTeacherBanCommand { Id = id, Ban = true });
        if (!result) return NotFound(new { message = "المدرس غير موجود" });
        return Ok(new { message = "تم حظر المدرس" });
    }

    [HttpPatch("{id}/unban")]
    public async Task<IActionResult> Unban(Guid id)
    {
        var result = await _mediator.Send(new ToggleTeacherBanCommand { Id = id, Ban = false });
        if (!result) return NotFound(new { message = "المدرس غير موجود" });
        return Ok(new { message = "تم فك الحظر بنجاح" });
    }
}