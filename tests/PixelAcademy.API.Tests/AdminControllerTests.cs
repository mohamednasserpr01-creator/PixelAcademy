using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Admin;
using PixelAcademy.Application.DTOs.AuditLogs;
using PixelAcademy.Application.DTOs.Common;
using Xunit;

namespace PixelAcademy.API.Tests;

public class AdminControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AdminControllerTests()
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
    public async Task GetDashboard_AsAdmin_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/admin/dashboard");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<AdminDashboardDto>();
        Assert.NotNull(result);
        Assert.True(result.UserStats.TotalUsers >= 3);
        Assert.True(result.CourseStats.TotalCourses >= 1);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetDashboard_AsInstructor_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/admin/dashboard");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task BanUser_AsAdmin_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var studentAuth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        var studentId = studentAuth.User.Id;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new { Reason = "Violation of terms" };
        var response = await _client.PostAsJsonAsync($"/api/admin/users/{studentId}/ban", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task BanUser_AsInstructor_Returns403()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "instructor@pixelacademy.com", "Instructor123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var studentAuth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        var studentId = studentAuth.User.Id;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new { Reason = "Test" };
        var response = await _client.PostAsJsonAsync($"/api/admin/users/{studentId}/ban", request);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task DisableCourse_AsAdmin_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var response = await _client.PostAsync($"/api/admin/courses/{courseId}/disable", null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task EnableCourse_AsAdmin_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var courseId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        await _client.PostAsync($"/api/admin/courses/{courseId}/disable", null);
        var response = await _client.PostAsync($"/api/admin/courses/{courseId}/enable", null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task DisableLecture_AsAdmin_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        var response = await _client.PostAsync($"/api/admin/lectures/{lectureId}/disable", null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task EnableLecture_AsAdmin_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var lectureId = Guid.Parse("66666666-6666-6666-6666-666666666666");
        await _client.PostAsync($"/api/admin/lectures/{lectureId}/disable", null);
        var response = await _client.PostAsync($"/api/admin/lectures/{lectureId}/enable", null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetAuditLogs_AsAdmin_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/auditlogs");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<AuditLogDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetAdminActions_AsAdmin_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/auditlogs/admin-actions");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<AuditLogDto>>();
        Assert.NotNull(result);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Moderation_AuditLogCreated()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var studentAuth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        var studentId = studentAuth.User.Id;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new { Reason = "Testing audit log" };
        await _client.PostAsJsonAsync($"/api/admin/users/{studentId}/ban", request);

        var logsResponse = await _client.GetAsync("/api/auditlogs");
        var logs = await logsResponse.Content.ReadApiResponseAsync<List<AuditLogDto>>();
        Assert.NotNull(logs);
        Assert.Contains(logs, l => l.Action == Domain.Enums.AuditActionType.BanUser);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Unauthorized_Access_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/admin/dashboard");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
