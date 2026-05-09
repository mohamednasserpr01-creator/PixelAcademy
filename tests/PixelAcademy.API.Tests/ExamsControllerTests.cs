using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Exams;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Domain.Enums;
using Xunit;

namespace PixelAcademy.API.Tests;

public class ExamsControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ExamsControllerTests()
    {
        _factory = new CustomWebApplicationFactory();
        _factory.SeedDatabaseAsync().GetAwaiter().GetResult();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task CreateExam_AsInstructor_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateExamRequestDto
        {
            Title = "New Exam",
            Description = "Test exam",
            Type = ExamType.CourseExam,
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            DurationMinutes = 20,
            AttemptLimit = 1,
            PassScorePercent = 50
        };

        var response = await _client.PostAsJsonAsync("/api/exams", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadApiResponseAsync<ExamDto>();
        Assert.NotNull(result);
        Assert.Equal("New Exam", result.Title);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task CreateExam_AsStudent_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateExamRequestDto
        {
            Title = "New Exam",
            Type = ExamType.CourseExam,
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            AttemptLimit = 1,
            PassScorePercent = 50
        };

        var response = await _client.PostAsJsonAsync("/api/exams", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetPublishedExams_Returns200()
    {
        var response = await _client.GetAsync("/api/exams");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<ExamDto>>();
        Assert.NotNull(result);
        Assert.Contains(result, e => e.Title == ".NET 8 Fundamentals Exam");
    }

    [Fact]
    public async Task GetExamById_Returns200()
    {
        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var response = await _client.GetAsync($"/api/exams/{examId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ExamDetailDto>();
        Assert.NotNull(result);
        Assert.Equal(".NET 8 Fundamentals Exam", result.Title);
        Assert.Equal(4, result.Questions.Count);
    }

    [Fact]
    public async Task StartExam_AsEnrolledStudent_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var response = await _client.PostAsync($"/api/exams/{examId}/start", null);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ExamAttemptDto>();
        Assert.NotNull(result);
        Assert.Equal(examId, result.ExamId);
        Assert.Equal(ExamAttemptStatus.InProgress, result.Status);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task StartExam_NotEnrolled_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var response = await _client.PostAsync($"/api/exams/{examId}/start", null);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task StartExam_AlreadyInProgress_Returns400()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var response1 = await _client.PostAsync($"/api/exams/{examId}/start", null);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        var response2 = await _client.PostAsync($"/api/exams/{examId}/start", null);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task StartExam_MaxAttemptsReached_Returns400()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");

        // Use both attempts
        for (int i = 0; i < 2; i++)
        {
            var startResponse = await _client.PostAsync($"/api/exams/{examId}/start", null);
            Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);
            var attempt = await startResponse.Content.ReadApiResponseAsync<ExamAttemptDto>();

            // Submit immediately to free up for next attempt
            var submitRequest = new SubmitExamAttemptRequestDto
            {
                ExamAttemptId = attempt!.Id,
                Answers = new List<SubmitAnswerRequestDto>()
            };
            await _client.PostAsJsonAsync($"/api/exams/attempts/{attempt.Id}/submit", submitRequest);
        }

        // Third attempt should fail
        var response3 = await _client.PostAsync($"/api/exams/{examId}/start", null);
        Assert.Equal(HttpStatusCode.BadRequest, response3.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task SubmitExam_AutoGradesCorrectly()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var startResponse = await _client.PostAsync($"/api/exams/{examId}/start", null);
        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);
        var attempt = await startResponse.Content.ReadApiResponseAsync<ExamAttemptDto>();
        Assert.NotNull(attempt);

        // Submit correct answers for q1 (C#), q2 (True), q3 (int, bool, double)
        var submitRequest = new SubmitExamAttemptRequestDto
        {
            ExamAttemptId = attempt.Id,
            Answers = new List<SubmitAnswerRequestDto>
            {
                new() { QuestionId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC1"), SelectedOptionId = "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD1" },
                new() { QuestionId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC2"), SelectedOptionId = "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD4" },
                new() { QuestionId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC3"), SelectedOptionIds = new List<string> { "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD6", "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD8", "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD9" } },
                new() { QuestionId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC4"), TextAnswer = "REST is lightweight; SOAP is protocol-heavy." }
            }
        };

        var submitResponse = await _client.PostAsJsonAsync($"/api/exams/attempts/{attempt.Id}/submit", submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var result = await submitResponse.Content.ReadApiResponseAsync<ExamResultDto>();
        Assert.NotNull(result);
        Assert.Equal(30, result.Score); // 10 + 10 + 10 + 0 (short answer not auto-graded)
        Assert.Equal(40, result.TotalPoints);
        Assert.True(result.IsPassed); // 30/40 = 75% >= 60% pass

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task SubmitExam_WrongAnswers_ReturnsLowerScore()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var startResponse = await _client.PostAsync($"/api/exams/{examId}/start", null);
        var attempt = await startResponse.Content.ReadApiResponseAsync<ExamAttemptDto>();
        Assert.NotNull(attempt);

        // Submit wrong answers
        var submitRequest = new SubmitExamAttemptRequestDto
        {
            ExamAttemptId = attempt.Id,
            Answers = new List<SubmitAnswerRequestDto>
            {
                new() { QuestionId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC1"), SelectedOptionId = "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD2" }, // Wrong
                new() { QuestionId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC2"), SelectedOptionId = "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD5" }, // Wrong
                new() { QuestionId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC3"), SelectedOptionIds = new List<string> { "DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD6" } } // Incomplete
            }
        };

        var submitResponse = await _client.PostAsJsonAsync($"/api/exams/attempts/{attempt.Id}/submit", submitRequest);
        Assert.Equal(HttpStatusCode.OK, submitResponse.StatusCode);
        var result = await submitResponse.Content.ReadApiResponseAsync<ExamResultDto>();
        Assert.NotNull(result);
        Assert.Equal(0, result.Score);
        Assert.False(result.IsPassed);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetMyResults_ReturnsResults()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var startResponse = await _client.PostAsync($"/api/exams/{examId}/start", null);
        var attempt = await startResponse.Content.ReadApiResponseAsync<ExamAttemptDto>();
        Assert.NotNull(attempt);

        var submitRequest = new SubmitExamAttemptRequestDto
        {
            ExamAttemptId = attempt.Id,
            Answers = new List<SubmitAnswerRequestDto>()
        };
        await _client.PostAsJsonAsync($"/api/exams/attempts/{attempt.Id}/submit", submitRequest);

        var resultsResponse = await _client.GetAsync("/api/exams/my-results");
        Assert.Equal(HttpStatusCode.OK, resultsResponse.StatusCode);
        var results = await resultsResponse.Content.ReadApiResponseAsync<List<ExamResultDto>>();
        Assert.NotNull(results);
        Assert.NotEmpty(results);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetExamAnalytics_AsInstructor_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var response = await _client.GetAsync($"/api/exams/{examId}/analytics");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<ExamAnalyticsDto>();
        Assert.NotNull(result);
        Assert.Equal(examId, result.ExamId);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetExamAnalytics_AsStudent_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var examId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
        var response = await _client.GetAsync($"/api/exams/{examId}/analytics");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Unauthorized_Access_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/exams/my-results");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
