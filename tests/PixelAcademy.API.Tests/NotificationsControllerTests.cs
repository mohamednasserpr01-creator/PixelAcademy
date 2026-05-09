using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PixelAcademy.Application.DTOs.Notifications;
using PixelAcademy.Domain.Enums;
using Xunit;

namespace PixelAcademy.API.Tests;

public class NotificationsControllerTests : IDisposable
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public NotificationsControllerTests()
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
    public async Task GetNotifications_Returns200()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/notifications");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<NotificationDto>>();
        Assert.NotNull(result);
        Assert.Contains(result, n => n.Title == "Welcome to PixelAcademy!");

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetUnreadNotifications_ReturnsOnlyUnread()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/notifications/unread");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<List<NotificationDto>>();
        Assert.NotNull(result);
        Assert.All(result, n => Assert.Equal(NotificationStatus.Unread, n.Status));

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task GetUnreadCount_ReturnsCorrectNumber()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.GetAsync("/api/notifications/unread-count");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var count = await response.Content.ReadApiResponseAsync<int>();
        Assert.True(count >= 1);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task MarkAsRead_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var notificationId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        var response = await _client.PostAsync($"/api/notifications/{notificationId}/mark-read", null);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify it's now read
        var getResponse = await _client.GetAsync($"/api/notifications/{notificationId}");
        var result = await getResponse.Content.ReadApiResponseAsync<NotificationDto>();
        Assert.NotNull(result);
        Assert.Equal(NotificationStatus.Read, result.Status);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task MarkAllAsRead_ReturnsCount()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var response = await _client.PostAsync("/api/notifications/mark-all-read", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var count = await response.Content.ReadApiResponseAsync<int>();
        Assert.True(count >= 1);

        // Verify unread count is now 0
        var countResponse = await _client.GetAsync("/api/notifications/unread-count");
        var unreadCount = await countResponse.Content.ReadApiResponseAsync<int>();
        Assert.Equal(0, unreadCount);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task CreateNotification_AsAdmin_Returns201()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "admin@pixelacademy.com", "Admin123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var studentAuth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        var studentId = studentAuth.User.Id;
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var request = new CreateNotificationRequestDto
        {
            UserId = studentId,
            Title = "New Notification",
            Message = "This is a test notification.",
            Type = NotificationType.General
        };

        var response = await _client.PostAsJsonAsync("/api/notifications", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadApiResponseAsync<NotificationDto>();
        Assert.NotNull(result);
        Assert.Equal("New Notification", result.Title);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task DeleteNotification_Returns204()
    {
        var auth = await TestAuthHelper.LoginAsync(_client, "student@pixelacademy.com", "Student123!");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        var notificationId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
        var response = await _client.DeleteAsync($"/api/notifications/{notificationId}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deleted
        var getResponse = await _client.GetAsync($"/api/notifications/{notificationId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);

        _client.DefaultRequestHeaders.Authorization = null;
    }

    [Fact]
    public async Task Unauthorized_Access_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/notifications");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
