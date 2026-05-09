using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.Assignments;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Application.Queries.Assignments;
using Asp.Versioning;
using System.Security.Claims;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
public class AssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<AssignmentDto>>> GetAssignments([FromQuery] Guid? courseId)
    {
        var result = await _mediator.Send(new GetAssignmentsQuery(courseId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<AssignmentDto>> GetAssignment(Guid id)
    {
        var result = await _mediator.Send(new GetAssignmentByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<AssignmentDto>> CreateAssignment(CreateAssignmentRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateAssignmentCommand(
            request.Title,
            request.Description,
            request.Instructions,
            request.CourseId,
            request.LectureId,
            request.DueDate,
            request.MaxPoints,
            request.AllowLateSubmission,
            userId
        );
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAssignment), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<AssignmentDto>> UpdateAssignment(Guid id, UpdateAssignmentRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new UpdateAssignmentCommand(
            id,
            request.Title,
            request.Description,
            request.Instructions,
            request.DueDate,
            request.MaxPoints,
            request.AllowLateSubmission,
            userId
        );
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> DeleteAssignment(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new DeleteAssignmentCommand(id, userId));
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<AssignmentDto>> PublishAssignment(Guid id, [FromQuery] bool published = true)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new PublishAssignmentCommand(id, published, userId));
        return Ok(result);
    }

    [HttpPost("submit")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<AssignmentSubmissionDto>> SubmitAssignment(SubmitAssignmentRequestDto request)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new SubmitAssignmentCommand(request.AssignmentId, studentId, request.TextAnswer, request.FileUrl);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMySubmission), new { assignmentId = request.AssignmentId }, result);
    }

    [HttpGet("{assignmentId:guid}/submissions")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<IReadOnlyList<AssignmentSubmissionDto>>> GetSubmissions(Guid assignmentId)
    {
        var result = await _mediator.Send(new GetAssignmentSubmissionsQuery(assignmentId));
        return Ok(result);
    }

    [HttpGet("{assignmentId:guid}/my-submission")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<AssignmentSubmissionDto>> GetMySubmission(Guid assignmentId)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new GetMySubmissionQuery(studentId, assignmentId));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("grade")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<AssignmentSubmissionDto>> GradeAssignment(GradeAssignmentRequestDto request)
    {
        var gradedByName = User.FindFirstValue(ClaimTypes.Name) ?? "Instructor";
        var command = new GradeAssignmentCommand(request.SubmissionId, request.Score, request.Feedback, gradedByName);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
