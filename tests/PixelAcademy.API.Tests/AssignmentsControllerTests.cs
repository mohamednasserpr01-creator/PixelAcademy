using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Assignments;
using PixelAcademy.Application.DTOs.ActivationCodes;
using Xunit;

namespace PixelAcademy.API.Tests;

public class AssignmentsControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AssignmentsControllerTests()
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
    public async Task CreateAssignment_AsInstructor_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateAssignmentRequestDto
        {
            Title = "New Assignment",
            Description = "Test assignment",
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            MaxPoints = 50,
            AllowLateSubmission = false
        };

        var response = await _client.PostAsJsonAsync("/api/assignments", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadApiResponseAsync<AssignmentDto>();
        Assert.NotNull(result);
        Assert.Equal("New Assignment", result.Title);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task CreateAssignment_AsStudent_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateAssignmentRequestDto
        {
            Title = "New Assignment",
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            MaxPoints = 50
        };

        var response = await _client.PostAsJsonAsync("/api/assignments", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetAssignments_Returns200()
    {
        var response = await _client.GetAsync("/api/assignments");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<AssignmentDto>>();
        Assert.NotNull(result);
        Assert.Contains(result, a => a.Title == "Build a Simple API");
    }

    [Fact]
    public async Task GetAssignmentById_Returns200()
    {
        var assignmentId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        var response = await _client.GetAsync($"/api/assignments/{assignmentId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<AssignmentDto>();
        Assert.NotNull(result);
        Assert.Equal("Build a Simple API", result.Title);
        Assert.Equal(100, result.MaxPoints);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task SubmitAssignment_AsEnrolledStudent_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        var assignmentId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        var request = new SubmitAssignmentRequestDto
        {
            AssignmentId = assignmentId,
            TextAnswer = "Here is my API implementation..."
        };

        var response = await _client.PostAsJsonAsync("/api/assignments/submit", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<AssignmentSubmissionDto>();
        Assert.NotNull(result);
        Assert.Equal(assignmentId, result.AssignmentId);
        Assert.Equal("Here is my API implementation...", result.TextAnswer);
        Assert.Null(result.Score);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task SubmitAssignment_NotEnrolled_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var assignmentId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        var request = new SubmitAssignmentRequestDto
        {
            AssignmentId = assignmentId,
            TextAnswer = "Here is my answer..."
        };

        var response = await _client.PostAsJsonAsync("/api/assignments/submit", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task SubmitAssignment_Duplicate_Returns400()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var assignmentId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        var request = new SubmitAssignmentRequestDto
        {
            AssignmentId = assignmentId,
            TextAnswer = "First submission"
        };
        var response1 = await _client.PostAsJsonAsync("/api/assignments/submit", request);
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);

        var response2 = await _client.PostAsJsonAsync("/api/assignments/submit", request);
        Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GradeAssignment_AsInstructor_Returns200()
    {
        var studentAuth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", studentAuth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var assignmentId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        var submitRequest = new SubmitAssignmentRequestDto
        {
            AssignmentId = assignmentId,
            TextAnswer = "My submission"
        };
        var submitResponse = await _client.PostAsJsonAsync("/api/assignments/submit", submitRequest);
        var submission = await submitResponse.Content.ReadApiResponseAsync<AssignmentSubmissionDto>();
        Assert.NotNull(submission);

        _client.DefaultRequestHeaders.Authorization = null;

        var instructorAuth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", instructorAuth.AccessToken);

        var gradeRequest = new GradeAssignmentRequestDto
        {
            SubmissionId = submission.Id,
            Score = 85,
            Feedback = "Good work, but could use more error handling."
        };

        var gradeResponse = await _client.PostAsJsonAsync("/api/assignments/grade", gradeRequest);
        Assert.Equal(HttpStatusCode.OK, gradeResponse.StatusCode);
        var result = await gradeResponse.Content.ReadApiResponseAsync<AssignmentSubmissionDto>();
        Assert.NotNull(result);
        Assert.Equal(85, result.Score);
        Assert.Equal("Good work, but could use more error handling.", result.Feedback);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GradeAssignment_AsStudent_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var gradeRequest = new GradeAssignmentRequestDto
        {
            SubmissionId = Guid.NewGuid(),
            Score = 100,
            Feedback = "Perfect!"
        };

        var response = await _client.PostAsJsonAsync("/api/assignments/grade", gradeRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetSubmissions_AsInstructor_Returns200()
    {
        var studentAuth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", studentAuth.AccessToken);

        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var assignmentId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        var submitRequest = new SubmitAssignmentRequestDto
        {
            AssignmentId = assignmentId,
            TextAnswer = "Student submission"
        };
        await _client.PostAsJsonAsync("/api/assignments/submit", submitRequest);
        _client.DefaultRequestHeaders.Authorization = null;

        var instructorAuth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", instructorAuth.AccessToken);

        var response = await _client.GetAsync($"/api/assignments/{assignmentId}/submissions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<AssignmentSubmissionDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetMySubmission_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);

        var assignmentId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        var submitRequest = new SubmitAssignmentRequestDto
        {
            AssignmentId = assignmentId,
            TextAnswer = "My answer"
        };
        await _client.PostAsJsonAsync("/api/assignments/submit", submitRequest);

        var response = await _client.GetAsync($"/api/assignments/{assignmentId}/my-submission");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<AssignmentSubmissionDto>();
        Assert.NotNull(result);
        Assert.Equal("My answer", result.TextAnswer);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Unauthorized_Access_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var assignmentId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE");
        var response = await _client.GetAsync($"/api/assignments/{assignmentId}/my-submission");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
