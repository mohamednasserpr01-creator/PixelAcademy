using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.WatchSessions;
using PixelAcademy.Application.DTOs.ActivationCodes;
using PixelAcademy.Application.DTOs.Progress;
using PixelAcademy.Application.DTOs.Analytics;
using PixelAcademy.Domain.Enums;
using Xunit;

namespace PixelAcademy.API.Tests;

public class WatchSessionsControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public WatchSessionsControllerTests()
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
    public async Task StartWatchSession_AsEnrolledStudent_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll student first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        var request = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };

        var response = await _client.PostAsJsonAsync("/api/watchsessions/start", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<WatchSessionDto>();
        Assert.NotNull(result);
        Assert.Equal(Guid.Parse("66666666-6666-6666-6666-666666666666"), result.LectureId);
        Assert.Equal(Guid.Parse("44444444-4444-4444-4444-444444444444"), result.CourseId);
        Assert.Equal("Getting Started with .NET 8", result.LectureTitle);
        Assert.Equal(0, result.DurationWatchedSeconds);
        Assert.False(result.IsCompleted);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task StartWatchSession_NotEnrolled_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };

        var response = await _client.PostAsJsonAsync("/api/watchsessions/start", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task StartWatchSession_PreviewLecture_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // First lecture is preview, no enrollment needed
        var request = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };

        var response = await _client.PostAsJsonAsync("/api/watchsessions/start", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task UpdateProgress_SavesCorrectly()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        // Start session
        var startRequest = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        var startResponse = await _client.PostAsJsonAsync("/api/watchsessions/start", startRequest);
        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);

        // Update progress at 15 minutes (900 seconds)
        var updateRequest = new UpdateWatchProgressRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CurrentPositionSeconds = 900,
            DurationWatchedSeconds = 900
        };
        var updateResponse = await _client.PostAsJsonAsync("/api/watchsessions/update-progress", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Check course progress
        var progressResponse = await _client.GetAsync($"/api/watchsessions/course-progress/{Guid.Parse("44444444-4444-4444-4444-444444444444")}");
        Assert.Equal(HttpStatusCode.OK, progressResponse.StatusCode);
        var progressResult = await progressResponse.Content.ReadApiResponseAsync<CourseProgressDto>();
        Assert.NotNull(progressResult);
        Assert.Equal(2, progressResult.TotalLectures);
        
        // 900 seconds out of 45 minutes (2700 seconds) = 33%
        var lectureProgress = progressResult.Lectures.First(l => l.LectureId == Guid.Parse("66666666-6666-6666-6666-666666666666"));
        Assert.True(lectureProgress.CompletionPercent >= 30);
        Assert.Equal(900, lectureProgress.LastPositionSeconds);
        Assert.False(lectureProgress.IsCompleted);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task FinishSession_MarksCompleted_AfterThreshold()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        // Start session
        var startRequest = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        var startResponse = await _client.PostAsJsonAsync("/api/watchsessions/start", startRequest);
        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);

        // Finish at 40 minutes (2400 seconds) out of 45 minutes (2700 seconds) = 89% > 85% threshold
        var finishRequest = new FinishWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            FinalPositionSeconds = 2400,
            TotalDurationWatchedSeconds = 2400
        };
        var finishResponse = await _client.PostAsJsonAsync("/api/watchsessions/finish", finishRequest);
        Assert.Equal(HttpStatusCode.OK, finishResponse.StatusCode);
        var finishResult = await finishResponse.Content.ReadApiResponseAsync<WatchSessionDto>();
        Assert.NotNull(finishResult);
        Assert.True(finishResult.IsCompleted);

        // Verify via course progress
        var progressResponse = await _client.GetAsync($"/api/watchsessions/course-progress/{Guid.Parse("44444444-4444-4444-4444-444444444444")}");
        var progressResult = await progressResponse.Content.ReadApiResponseAsync<CourseProgressDto>();
        Assert.NotNull(progressResult);
        var lectureProgress = progressResult.Lectures.First(l => l.LectureId == Guid.Parse("66666666-6666-6666-6666-666666666666"));
        Assert.True(lectureProgress.IsCompleted);
        Assert.True(lectureProgress.CompletionPercent >= 85);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetSignedVideoUrl_AsEnrolledStudent_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        var response = await _client.PostAsync($"/api/watchsessions/signed-url/{Guid.Parse("66666666-6666-6666-6666-666666666666")}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<SignedVideoUrlDto>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.SignedUrl);
        Assert.Equal("Getting Started with .NET 8", result.LectureTitle);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetSignedVideoUrl_NotEnrolled_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.PostAsync($"/api/watchsessions/signed-url/{Guid.Parse("77777777-7777-7777-7777-777777777777")}", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task ContinueWatching_ReturnsLatestSession()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        // Start session
        var startRequest = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        var startResponse = await _client.PostAsJsonAsync("/api/watchsessions/start", startRequest);
        Assert.Equal(HttpStatusCode.Created, startResponse.StatusCode);

        // Update progress
        var updateRequest = new UpdateWatchProgressRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CurrentPositionSeconds = 1200,
            DurationWatchedSeconds = 1200
        };
        var updateResponse = await _client.PostAsJsonAsync("/api/watchsessions/update-progress", updateRequest);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        // Get continue watching
        var continueResponse = await _client.GetAsync("/api/watchsessions/continue");
        Assert.Equal(HttpStatusCode.OK, continueResponse.StatusCode);
        var continueResult = await continueResponse.Content.ReadApiResponseAsync<ContinueWatchingDto>();
        Assert.NotNull(continueResult);
        Assert.Equal(Guid.Parse("66666666-6666-6666-6666-666666666666"), continueResult.LectureId);
        Assert.Equal("Getting Started with .NET 8", continueResult.LectureTitle);
        Assert.Equal(1200, continueResult.LastPositionSeconds);
        Assert.True(continueResult.CompletionPercent >= 40);
        Assert.False(continueResult.IsCompleted);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetCompletedLectures_ReturnsOnlyCompleted()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        // Start, update, finish first lecture to completion
        var startRequest = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        await _client.PostAsJsonAsync("/api/watchsessions/start", startRequest);

        var finishRequest = new FinishWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            FinalPositionSeconds = 2400,
            TotalDurationWatchedSeconds = 2400
        };
        await _client.PostAsJsonAsync("/api/watchsessions/finish", finishRequest);

        // Get completed lectures
        var completedResponse = await _client.GetAsync("/api/watchsessions/completed");
        Assert.Equal(HttpStatusCode.OK, completedResponse.StatusCode);
        var completedResult = await completedResponse.Content.ReadApiResponseAsync<List<LectureProgressSummaryDto>>();
        Assert.NotNull(completedResult);
        Assert.Single(completedResult);
        Assert.Equal(Guid.Parse("66666666-6666-6666-6666-666666666666"), completedResult[0].LectureId);
        Assert.True(completedResult[0].IsCompleted);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetCourseProgress_ReturnsOverallPercent()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        // Enroll first
        var enrollRequest = new RedeemActivationCodeRequestDto { Code = "PA-ENROLL-TEST01" };
        var enrollResponse = await _client.PostAsJsonAsync("/api/activationcodes/redeem", enrollRequest);
        Assert.Equal(HttpStatusCode.OK, enrollResponse.StatusCode);

        // Complete one lecture
        var startRequest = new StartWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            CourseId = Guid.Parse("44444444-4444-4444-4444-444444444444")
        };
        await _client.PostAsJsonAsync("/api/watchsessions/start", startRequest);

        var finishRequest = new FinishWatchSessionRequestDto
        {
            LectureId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            FinalPositionSeconds = 2400,
            TotalDurationWatchedSeconds = 2400
        };
        await _client.PostAsJsonAsync("/api/watchsessions/finish", finishRequest);

        // Get course progress
        var progressResponse = await _client.GetAsync($"/api/watchsessions/course-progress/{Guid.Parse("44444444-4444-4444-4444-444444444444")}");
        Assert.Equal(HttpStatusCode.OK, progressResponse.StatusCode);
        var progressResult = await progressResponse.Content.ReadApiResponseAsync<CourseProgressDto>();
        Assert.NotNull(progressResult);
        Assert.Equal(2, progressResult.TotalLectures);
        Assert.Equal(1, progressResult.CompletedLectures);
        Assert.True(progressResult.OverallCompletionPercent > 40);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Unauthorized_Access_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync("/api/watchsessions/continue");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
