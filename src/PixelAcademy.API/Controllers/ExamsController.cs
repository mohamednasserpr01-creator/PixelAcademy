using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PixelAcademy.Application.Commands.ExamAttempts;
using PixelAcademy.Application.Commands.Exams;
using PixelAcademy.Application.Commands.Questions;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Application.Queries.Exams;
using PixelAcademy.Domain.Enums;
using Asp.Versioning;
using System.Security.Claims;

namespace PixelAcademy.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<ExamDto>>> GetExams([FromQuery] Guid? courseId)
    {
        var result = await _mediator.Send(new GetExamsQuery(courseId));
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ExamDetailDto>> GetExam(Guid id)
    {
        var result = await _mediator.Send(new GetExamByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<ExamDto>> CreateExam(CreateExamRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new CreateExamCommand(
            request.Title,
            request.Description,
            request.Type,
            request.CourseId,
            request.LectureId,
            request.DurationMinutes,
            request.AttemptLimit,
            request.PassScorePercent,
            request.IsRandomized,
            request.StartDate,
            request.EndDate,
            request.ShowCorrectAnswers,
            userId
        );
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetExam), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<ExamDto>> UpdateExam(Guid id, UpdateExamRequestDto request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new UpdateExamCommand(
            id,
            request.Title,
            request.Description,
            request.DurationMinutes,
            request.AttemptLimit,
            request.PassScorePercent,
            request.IsRandomized,
            request.StartDate,
            request.EndDate,
            request.ShowCorrectAnswers,
            userId
        );
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> DeleteExam(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(new DeleteExamCommand(id, userId));
        return NoContent();
    }

    [HttpPost("{id:guid}/publish")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<ExamDto>> PublishExam(Guid id, [FromQuery] bool published = true)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new PublishExamCommand(id, published, userId));
        return Ok(result);
    }

    // Questions
    [HttpGet("{examId:guid}/questions")]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<QuestionDto>>> GetQuestions(Guid examId)
    {
        var result = await _mediator.Send(new GetExamQuestionsQuery(examId));
        return Ok(result);
    }

    [HttpPost("{examId:guid}/questions")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<QuestionDto>> CreateQuestion(Guid examId, CreateQuestionRequestDto request)
    {
        var command = new CreateQuestionCommand(
            examId,
            request.Text,
            request.Explanation,
            request.Type,
            request.OrderIndex,
            request.Points,
            request.Options.Select(o => new CreateQuestionOptionRequest(o.Text, o.OrderIndex, o.IsCorrect)).ToList()
        );
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetQuestions), new { examId }, result);
    }

    [HttpPut("questions/{questionId:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<QuestionDto>> UpdateQuestion(Guid questionId, UpdateQuestionRequestDto request)
    {
        var command = new UpdateQuestionCommand(
            questionId,
            request.Text,
            request.Explanation,
            request.OrderIndex,
            request.Points,
            request.Options.Select(o => new CreateQuestionOptionRequest(o.Text, o.OrderIndex, o.IsCorrect)).ToList()
        );
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("questions/{questionId:guid}")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<IActionResult> DeleteQuestion(Guid questionId)
    {
        await _mediator.Send(new DeleteQuestionCommand(questionId));
        return NoContent();
    }

    // Exam Attempts
    [HttpPost("{examId:guid}/start")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ExamAttemptDto>> StartExam(Guid examId)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new StartExamAttemptCommand(examId, studentId));
        return CreatedAtAction(nameof(GetAttempt), new { attemptId = result.Id }, result);
    }

    [HttpGet("attempts/{attemptId:guid}")]
    [Authorize]
    public async Task<ActionResult<ExamAttemptDto>> GetAttempt(Guid attemptId)
    {
        var result = await _mediator.Send(new GetExamAttemptQuery(attemptId));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("attempts/{attemptId:guid}/submit")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<ExamResultDto>> SubmitAttempt(Guid attemptId, SubmitExamAttemptRequestDto request)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var command = new SubmitExamAttemptCommand(
            attemptId,
            request.Answers.Select(a => new SubmitAnswerRequest(a.QuestionId, a.SelectedOptionId, a.SelectedOptionIds, a.TextAnswer)).ToList(),
            studentId
        );
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("my-results")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<IReadOnlyList<ExamResultDto>>> GetMyResults([FromQuery] Guid? examId)
    {
        var studentId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(new GetStudentExamResultsQuery(studentId, examId));
        return Ok(result);
    }

    [HttpGet("{examId:guid}/analytics")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<ActionResult<ExamAnalyticsDto>> GetAnalytics(Guid examId)
    {
        var result = await _mediator.Send(new GetExamAnalyticsQuery(examId));
        return Ok(result);
    }
}
