using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Application.DTOs.Analytics;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Domain.Enums;
using Xunit;

namespace PixelAcademy.API.Tests;

public class AnalyticsControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AnalyticsControllerTests()
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
    public async Task GetLectureAnalytics_AsInstructor_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var response = await _client.GetAsync($"/api/analytics/lecture/{lectureId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<LectureAnalyticsDto>();
        Assert.NotNull(result);
        Assert.Equal("Getting Started with .NET 8", result.LectureTitle);
        Assert.Equal(0, result.TotalWatchTimeSeconds); // No sessions yet
        Assert.Equal(0, result.UniqueWatchers);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetCourseAnalytics_AsInstructor_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var response = await _client.GetAsync($"/api/analytics/course/{courseId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<CourseAnalyticsDto>();
        Assert.NotNull(result);
        Assert.Equal("Introduction to .NET 8", result.CourseTitle);
        Assert.Equal(0, result.TotalEnrolledStudents); // Seed has no enrollments
        Assert.Equal(0, result.TotalWatchTimeSeconds);
        Assert.Equal(2, result.Lectures.Count);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetCourseAnalytics_AfterWatchActivity_ReturnsCorrectData()
    {
        // Student enrolls and watches a lecture
        var studentAuth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", studentAuth.AccessToken);

        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        // Start and finish a watch session
        var startRequest = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        var startResponse = await _client.PostAsJsonAsync("/api/watchsessions/start", startRequest);
        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);

        var finishRequest = new FinishWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            FinalPositionSeconds = 2400,
            TotalDurationWatchedSeconds = 2400
        };
        var finishResponse = await _client.PostAsJsonAsync("/api/watchsessions/finish", finishRequest);
        Assert.Equal(HttpStatusCode.OK, finishResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;

        // Instructor checks analytics
        var instructorAuth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", instructorAuth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var response = await _client.GetAsync($"/api/analytics/course/{courseId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadApiResponseAsync<CourseAnalyticsDto>();
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalEnrolledStudents);
        Assert.Equal(2400, result.TotalWatchTimeSeconds);
        Assert.True(result.AverageCourseCompletionPercent > 40);
        Assert.Equal(2, result.Lectures.Count);

        var firstLecture = result.Lectures.First(l => l.LectureId == Guid.Parse("66666666-6666-6666-6666-666666666666"));
        Assert.Equal(2400, firstLecture.TotalWatchTimeSeconds);
        Assert.Equal(1, firstLecture.UniqueWatchers);
        Assert.Equal(1, firstLecture.CompletionCount);
        Assert.True(firstLecture.AverageCompletionPercent >= 85);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetAnalytics_AsStudent_Fails()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var response = await _client.GetAsync($"/api/analytics/lecture/{lectureId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetAnalytics_Unauthorized_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var response = await _client.GetAsync($"/api/analytics/lecture/{lectureId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
